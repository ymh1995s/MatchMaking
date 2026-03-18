namespace MatchMaking.Models
{
    public class MatchResult
    {
        public bool IsMatched { get; set; } // 자신이 정원의 마지막이면 true(게임 시작), 아니면 false(다른 유저 대기)
        public List<UserInfo> Members { get; set; } = new();
        public int WaitingCount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty; // 서버에서 주고싶은 메시지
    }
}
