using Microsoft.AspNetCore.SignalR;
using SC_DataSimulator.DomainModels;
using SC_DataSimulator.Models;

namespace SC_DataSimulator
{
    public class DriverHub : Hub
    {
        private readonly IHubContext<DriverHub> _hubContext;

        public DriverHub(IHubContext<DriverHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ActivityUpdated(List<TotalHoursType> totalHoursTypes)
        {
            await _hubContext.Clients.All.SendAsync("ActivityUpdated", totalHoursTypes);
        }

        public async Task SingleDriveTimeViolation(DriverSingleViolation driverSingleViolation)
        {
            await _hubContext.Clients.All.SendAsync("SingeDriveTimeViolation", driverSingleViolation);
        }
    }
}
