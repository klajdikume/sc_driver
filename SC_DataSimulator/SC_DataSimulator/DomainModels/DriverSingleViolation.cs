namespace SC_DataSimulator.DomainModels
{
    public class DriverSingleViolation
    {
        public int DriveTime { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int Limit { get; set; }
    }
}
