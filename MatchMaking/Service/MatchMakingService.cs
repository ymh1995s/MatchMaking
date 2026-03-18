using MatchMaking.Hubs;
using MatchMaking.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MatchMaking.Service
{
    public class MatchMakingService
    {
        const int MAX_PLAYERS = 4;
        private readonly object _lock = new(); // ex1) 4명이 정원인 게임에서 서로 다른 스레드가 4번째 유저의 입장을 받을 때 경쟁
        private readonly Dictionary<Tier, List<UserInfo>> _waitingPool = new(); // Tier별 대기 유저 목록, 쉬운 취소를 위해 Dic. 사용

        private readonly IHubContext<MatchMakingHub> _hubContext;

        public MatchMakingService(IHubContext<MatchMakingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // 봇 전용 조인 - 대기열에만 추가하고 매칭 성사 체크 안 함 (봇끼리 매칭 방지)
        public async Task JoinBotAsync(UserInfo botInfo)
        {
            await Task.Delay(10);

            lock (_lock)
            {
                if (!_waitingPool.ContainsKey(botInfo.Tier))
                    _waitingPool[botInfo.Tier] = new List<UserInfo>();

                _waitingPool[botInfo.Tier].Add(botInfo);
            }
        }

        // 매칭 참가
        public async Task<MatchResult> JoinAsync(UserInfo userInfo)
        {
            // 실제 매칭 로직을 가정한 비동기 지연 (ex. 나중에 DB 조회, 외부 서비스 호출 등이 추가될 수 있음)
            await Task.Delay(1000);

            MatchResult result;

            lock (_lock)
            {
                // 모든 티어 대기열에서 동일 Id 중복 확인
                bool isDuplicate = _waitingPool.Values.Any(list => list.Any(u => u.Id == userInfo.Id));
                if (isDuplicate)
                {
                    return new MatchResult
                    {
                        IsDuplicate = true,
                        Message = $"{userInfo.Id} 는 이미 대기열에 등록되어 있습니다."
                    };
                }

                if (!_waitingPool.ContainsKey(userInfo.Tier))
                    _waitingPool[userInfo.Tier] = new List<UserInfo>(); // C#은 key 없을 때 value 자동 생성 없어서 명시적 List 생성
                var list = _waitingPool[userInfo.Tier];
                list.Add(userInfo);

                // N명 모이면 매칭 성사
                if (list.Count >= MAX_PLAYERS)
                {
                    // 요청한 플레이어(Human)를 반드시 포함시키고 나머지 MAX_PLAYERS-1명을 채움
                    // 이 토이프로젝트에서만 이 로직인거고 실제 서비스에서는 다르게 구현
                    var others = list.Where(u => u.Id != userInfo.Id).Take(MAX_PLAYERS - 1).ToList();
                    var members = others.Prepend(userInfo).ToList();

                    // 매칭된 N명을 대기열에서 제거
                    foreach (var m in members)
                        list.Remove(m);

                    result = new MatchResult
                    {
                        IsMatched = true,
                        Members = members,
                        Message = $"Match found! Players: {string.Join(", ", members.Select(m => m.Id))}"
                    };
                }
                else
                {
                    result = new MatchResult
                    {
                        IsMatched = false,
                        WaitingCount = list.Count,
                        Message = $"Waiting for more players... ({list.Count}/{MAX_PLAYERS})"
                    };
                }
            }

            // SignalR 호출 부분
            // lock 안에서 async/await 불가

            var tierGroup = userInfo.Tier.ToString();

            if (result.IsMatched)
            {
                // ▼ 매칭 성사! 해당 Tier 그룹 전체에게 매칭 완료 알림
                await _hubContext.Clients.Group(tierGroup)
                    .SendAsync("MatchComplete", result);
            }
            else
            {
                // ▼ 새 유저 참가 알림: 해당 Tier 그룹 전체에게 현재 대기 인원수를 Push
                await _hubContext.Clients.Group(tierGroup)
                    .SendAsync("WaitingCountUpdated", result.WaitingCount, MAX_PLAYERS);
            }

            return result;
        }


        // 매칭 취소
        public async Task<MatchResult?> CancelAsync(string userId)
        {
            // 취소 처리를 가정한 비동기 지연
            await Task.Delay(1000);

            Tier? cancelledTier = null;
            int remainingCount = 0;

            lock (_lock)
            {
                foreach (var (tier, list) in _waitingPool)
                {
                    var user = list.Find(u => u.Id == userId);
                    if (user != null)
                    {
                        list.Remove(user);
                        cancelledTier = tier;
                        remainingCount = list.Count;
                        break;
                    }
                }
            }

            if (cancelledTier is null)
                return null;

            // 취소 후 남은 대기 인원을 해당 Tier 그룹에 Push
            await _hubContext.Clients.Group(cancelledTier.ToString())
                .SendAsync("WaitingCountUpdated", remainingCount, MAX_PLAYERS);

            return new MatchResult
            {
                IsMatched = false,
                WaitingCount = remainingCount,
                Message = $"{userId} 매칭 취소 완료. 현재 대기 인원: {remainingCount}/{MAX_PLAYERS}"
            };
        }
    }
}
