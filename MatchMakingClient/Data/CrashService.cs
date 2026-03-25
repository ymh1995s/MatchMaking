namespace MatchMakingClient.Data
{
    // 덤프 테스트용 크래쉬 API 호출 서비스
    public class CrashService
    {
        private readonly HttpClient _httpClient;

        public CrashService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST api/crash 요청을 보내 서버를 강제 크래쉬시킴
        public async Task<string> CrashAsync()
        {
            var response = await _httpClient.PostAsync("api/crash", null);
            return response.IsSuccessStatusCode
                ? "크래쉬 요청 전송 완료"
                : $"요청 실패: {response.StatusCode}";
        }
    }
}
