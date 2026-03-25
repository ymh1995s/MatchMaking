using Microsoft.AspNetCore.Mvc;

namespace MatchMaking.Controllers
{
    // 덤프 테스트용 크래쉬 API 컨트롤러
    [ApiController]
    [Route("api/[controller]")]
    public class CrashController : ControllerBase
    {
        private readonly ILogger<CrashController> _logger;

        public CrashController(ILogger<CrashController> logger)
        {
            _logger = logger;
        }

        // POST api/crash
        // 호출 즉시 서버 프로세스를 강제 크래쉬시킴 (덤프 테스트용)
        [HttpPost]
        public IActionResult Crash()
        {
            _logger.LogCritical("크래쉬 API 호출됨 - 프로세스를 강제 종료합니다.");

            // Environment.FailFast는 즉시 프로세스를 종료하고 덤프를 생성할 수 있도록 함
            // 일반 예외와 달리 catch 블록이나 finally 블록이 실행되지 않음
            Environment.FailFast("덤프 테스트용 강제 크래쉬");

            // 실제로 도달하지 않지만 컴파일러를 위해 반환
            return Ok();
        }
    }
}
