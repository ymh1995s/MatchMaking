using Microsoft.AspNetCore.Mvc;

namespace MatchMaking.Controllers
{
    // REST API 컨트롤러 - api/ping 경로로 들어오는 요청을 처리
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        // GET api/ping/ping
        // 비동기 처리 연습용 핑 엔드포인트
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            // 실제 업무 처리를 가정한 2초 인위적 지연
            // Thread.Sleep과 달리 await Task.Delay는 스레드를 점유하지 않아
            // 대기 중에도 서버가 다른 요청을 처리할 수 있음
            await Task.Delay(2000);

            // 응답 메시지와 서버 현재 시각(UTC)을 JSON으로 반환
            return Ok(new
            {
                Message = "Pong!",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
