using global::MatchMaking.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MatchMakingClient.Data
{
    // API 서버의 MatchMaking 엔드포인트와의 HTTP 통신을 담당하는 서비스 클래스
    // Startup.cs에서 DI 컨테이너에 등록되어 MatchRequest.razor에서 주입받아 사용됨
    public class MatchRequestService
    {
        // Startup.cs에서 BaseAddress(API 서버 URL)가 설정된 HttpClient를 주입받음
        private readonly HttpClient _httpClient;

        // 생성자 주입 - DI 컨테이너가 HttpClient를 자동으로 제공
        public MatchRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST api/matchmaking 요청을 비동기로 보냄
        // UserInfo 객체를 JSON으로 직렬화하여 전송
        public async Task<MatchResult> JoinMatchAsync(UserInfo userInfo)
        {
            var response = await _httpClient.PostAsJsonAsync("api/matchmaking", userInfo);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MatchResult>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
            }
        }

        // DELETE api/matchmaking/{id} 요청을 비동기로 보냄
        // 사용자 ID로 대기열에서 제거
        public async Task<MatchResult> CancelMatchAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"api/matchmaking/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MatchResult>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
            }
        }
    }
}
