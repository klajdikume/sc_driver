using System.Threading;

namespace SC_DataSimulator
{
    public class SimulatedData
    {
        public DateTime Timestamp { get; set; }
        public string Activity { get; set; } = string.Empty;
    }

    public class SimulatedDataGenerator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cancellationTokenSource;

        public SimulatedDataGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var simulatedData = new SimulatedData
                {
                    Timestamp = DateTime.UtcNow,
                    Activity = GetRandomActivity(random)
                };

                // Store or process simulated data as needed
                Console.WriteLine($"Generated: {simulatedData.Timestamp} - {simulatedData.Activity}");

                // Adjust delay based on the desired data generation rate
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            _cancellationTokenSource.TryReset();
        }

        private string GetRandomActivity(Random random)
        {
            // Generate random activity
            string[] activities = { "Driving", "Resting", "Break", "Eating", "Sleeping" };
            return activities[random.Next(activities.Length)];
        }
    }

}
