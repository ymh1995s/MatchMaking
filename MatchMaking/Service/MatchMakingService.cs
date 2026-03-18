using MatchMaking.Models;
using System.Collections.Concurrent;

namespace MatchMaking.Service
{
    public class MatchMakingService
    {
        const int MAX_PLAYERS = 4;
        private readonly object _lock = new(); // ex1) 4명이 정원인 게임에서 서로 다른 스레드가 4번째 유저의 입장을 받을 때 경쟁
        private readonly Dictionary<Tier, List<UserInfo>> _waitingPool = new(); // Tier별 대기 유저 목록, 쉬운 취소를 위해 Dic. 사용

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

            lock (_lock)
            {
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

                    return new MatchResult
                    {
                        IsMatched = true,
                        Members = members,
                        Message = $"Match found! Players: {string.Join(", ", members.Select(m => m.Id))}"
                    };
                }

                // 아직 정원이 안 찼으면 대기하라고 결과 보냄
                return new MatchResult
                {
                    IsMatched = false,
                    WaitingCount = list.Count,
                    Message = $"Waiting for more players... ({list.Count}/{MAX_PLAYERS})"
                };
            }
        }


        // 매칭 취소
        public async Task<bool> CancelAsync(string userId)
        {
            // 취소 처리를 가정한 비동기 지연
            await Task.Delay(1000);

            lock (_lock)
            {
                foreach (var list in _waitingPool.Values)
                {
                    var user = list.Find(u => u.Id == userId);
                    if (user != null)
                    {
                        list.Remove(user);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
