using MatchMaking.Models;
using MatchMaking.Service;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchMakingController : ControllerBase
    {
        private readonly MatchMakingService _matchMakingService;

        // 생성자 주입 - DI 컨테이너가 MatchMakingService를 자동으로 제공
        public MatchMakingController(MatchMakingService matchMakingService)
        {
            _matchMakingService = matchMakingService;
        }

        // POST api/matchmaking
        // 매칭 참가 요청
        [HttpPost]
        public async Task<IActionResult> JoinMatch([FromBody] UserInfo userInfo)
        {
            var result = await _matchMakingService.JoinAsync(userInfo);
            return Ok(result);
        }

        // DELETE api/matchmaking/{id}
        // 매칭 취소 요청
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelMatch(string id)
        {
            var cancelled = await _matchMakingService.CancelAsync(id);

            if (cancelled) return Ok(new { Message = $"{id} 매칭 취소 완료", Cancelled = true });
            return NotFound(new { Message = $"{id} 대기열에 없음 (이미 매칭됐거나 잘못된 ID)", Cancelled = false });
        }
    }
}
