using System;

namespace MatchMakingClient.Data
{
    // API 서버(PingController)의 Ping 응답 JSON을 담는 데이터 모델
    // {"message": "Pong!", "timestamp": "..."} 형태의 JSON과 1:1 매핑됨
    public class PingResult
    {
        // 서버에서 보내는 응답 메시지 (예: "Pong!")
        public string Message { get; set; }

        // 서버에서 응답을 보낸 시각 (UTC 기준)
        public DateTime Timestamp { get; set; }
    }
}
