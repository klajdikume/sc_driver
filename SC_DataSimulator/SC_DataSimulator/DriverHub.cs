using Microsoft.AspNetCore.SignalR;
using SC_DataSimulator.Models;

namespace SC_DataSimulator
{
    public class DriverHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("DriverUpdated", $"{Context.ConnectionId}");
        }

        public async Task DriverUpdated(Activity activity)
        {
            await Clients.All.SendAsync("DriverUpdated", activity);
        }
    }
}
