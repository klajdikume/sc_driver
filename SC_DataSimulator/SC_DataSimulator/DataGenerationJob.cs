

using Microsoft.AspNetCore.SignalR;
using SC_DataSimulator.Models;
using SC_DataSimulator.DAL;
using System;
using SC_DataSimulator.DomainModels;

namespace SC_DataSimulator
{
    public class DataGenerationJob
    {
        private readonly ILogger<DataGenerationJob> _logger;
        private readonly IDriverActivity _driverActivity;
        private readonly IHubContext<DriverHub> _hubContext;
        private static readonly Random random = new Random();
        private static string[] activityTypes = { "Driving", "Resting" };

        public DataGenerationJob(ILogger<DataGenerationJob> logger, IDriverActivity driverActivity, IHubContext<DriverHub> hubContext)
        {
            _logger = logger;
            _driverActivity = driverActivity;
            _hubContext = hubContext;
        }

        private static string GetRandomActivityType()
        {
            return activityTypes[random.Next(activityTypes.Length)];
        }

        public async Task Execute()
        {
            _logger.LogInformation("Data generation job started.");

            if(!_driverActivity.ReachedMaxToday())
            {
                var activity = new Activity
                {
                    Type = GetRandomActivityType(),
                    DriverId = 1,
                    Date = new DateTime(2024, 04, 09),
                    Start = DateTime.Now + TimeSpan.FromHours(random.Next(1, 4)),
                    End = DateTime.Now + TimeSpan.FromHours(random.Next(1, 4)),
                };

                activity.End = activity.End > activity.Start ? activity.End : activity.Start.AddHours(random.Next(1, 8));

                // TODO:
                // check the constraint of overlap possibility

                _driverActivity.AddActivity(activity);

                DriverDashboard driverDashboard = new DriverDashboard(); 
                driverDashboard.DriverSingleViolation = _driverActivity.DriversSingleDriveViolation();
                driverDashboard.TotalHours = _driverActivity.TotalDriveHoursTypes();
                // Driverswithviolations

                await _hubContext.Clients.All.SendAsync("DriverUpdated", driverDashboard);
            }

            await Task.Delay(TimeSpan.FromSeconds(1)); 

            _logger.LogInformation("Data generation job completed.");
        }
    }
}
