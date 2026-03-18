using MatchMaking.Models;
using MatchMaking.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MatchMakingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 봇 100명 생성 및 대기열 등록
// Tier1, Tier10 = 0명 / Tier2~Tier9 = 정규분포(평균=Tier5.5, 표준편차=1.5) 기반 100명 배분
await SeedBotsAsync(app.Services);

app.Run();

static async Task SeedBotsAsync(IServiceProvider services)
{
    var matchMakingService = services.GetRequiredService<MatchMakingService>();

    // Tier2(index 0) ~ Tier9(index 7) 에 대해 정규분포 가중치 계산
    // 티어 번호: 2~9, 평균=5.5, 표준편차=1.5
    const double mean = 5.5;
    const double stdDev = 1.5;
    const int totalBots = 100;

    var tiers = new[]
    {
        Tier.Tier2, Tier.Tier3, Tier.Tier4, Tier.Tier5,
        Tier.Tier6, Tier.Tier7, Tier.Tier8, Tier.Tier9
    };

    // 각 티어의 정규분포 밀도값 계산
    double[] weights = tiers
        .Select((t, i) =>
        {
            double x = i + 2; // Tier2=2, Tier3=3, ..., Tier9=9
            double exponent = -0.5 * Math.Pow((x - mean) / stdDev, 2);
            return Math.Exp(exponent);
        })
        .ToArray();

    double weightSum = weights.Sum();

    // 가중치 비율에 따라 봇 수 배분 (반올림 후 합계가 100이 되도록 보정)
    int[] botCounts = new int[tiers.Length];
    int assigned = 0;
    for (int i = 0; i < tiers.Length - 1; i++)
    {
        botCounts[i] = (int)Math.Round(weights[i] / weightSum * totalBots);
        assigned += botCounts[i];
    }
    // 마지막 티어에 나머지 배정 (반올림 오차 보정)
    botCounts[tiers.Length - 1] = totalBots - assigned;

    // 봇 생성 및 대기열 등록
    int botIndex = 1;
    for (int i = 0; i < tiers.Length; i++)
    {
        for (int j = 0; j < botCounts[i]; j++)
        {
            var bot = new UserInfo
            {
                Id = $"Bot_{botIndex:D3}",
                Tier = tiers[i],
                MatchState = MatchState.Matching,
                HumanType = HumanType.Bot
            };
            await matchMakingService.JoinBotAsync(bot);
            botIndex++;
        }
    }

    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("봇 {TotalBots}명 대기열 등록 완료", totalBots);
    for (int i = 0; i < tiers.Length; i++)
        logger.LogInformation("  {Tier}: {Count}명", tiers[i], botCounts[i]);
}
