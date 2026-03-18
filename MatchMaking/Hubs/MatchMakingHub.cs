using Microsoft.AspNetCore.SignalR;

namespace MatchMaking.Hubs
{
    /// <summary>
    /// SignalR 허브 클래스 - 클라이언트와 실시간 통신을 담당
    /// </summary>
    public class MatchMakingHub : Hub
    {
        public async Task JoinTierGroup(string tier)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, tier);
        }

        public async Task LeaveTierGroup(string tier)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tier);
        }
    }
}
