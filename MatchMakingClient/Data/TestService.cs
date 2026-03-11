using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MatchMakingClient.Data
{
    // API 서버와의 HTTP 통신 로직을 담당하는 서비스 클래스
    // Startup.cs에서 DI 컨테이너에 등록되어 Test.razor에서 주입받아 사용됨
    public class TestService
    {
        // Startup.cs에서 BaseAddress(API 서버 URL)가 설정된 HttpClient를 주입받음
        private readonly HttpClient _httpClient;

        // 생성자 주입 - DI 컨테이너가 HttpClient를 자동으로 제공
        public TestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET api/test/ping 요청을 비동기로 보내고 결과를 PingResult로 반환
        // await 키워드로 HTTP 응답을 기다리는 동안 스레드를 블로킹하지 않음
        public async Task<PingResult> PingAsync()
        {
            // HTTP GET 요청 → JSON 응답 → PingResult 객체로 자동 역직렬화
            return await _httpClient.GetFromJsonAsync<PingResult>("api/test/ping");
        }
    }
}
