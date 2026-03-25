using MatchMakingClient.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace MatchMakingClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // appsettings.json의 ApiSettings:BaseUrl 값을 읽어와 API 서버 주소로 설정
            var baseUrl = Configuration["ApiSettings:BaseUrl"];
            // PingService 전용 HttpClient를 DI 컨테이너에 등록
            // AddHttpClient를 쓰면 HttpClient의 생명주기를 프레임워크가 관리해줌 (직접 new 하지 않음)
            services.AddHttpClient<PingService>(client =>
            {
                // 모든 HTTP 요청의 기본 주소를 API 서버 URL로 고정
                // 이후 PingService에서는 "api/ping/ping" 처럼 상대 경로만 사용하면 됨
                client.BaseAddress = new Uri(baseUrl);
            });

            // MatchRequestService 전용 HttpClient를 DI 컨테이너에 등록
            services.AddHttpClient<MatchRequestService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            // CrashService 전용 HttpClient를 DI 컨테이너에 등록 (덤프 테스트용)
            services.AddHttpClient<CrashService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
