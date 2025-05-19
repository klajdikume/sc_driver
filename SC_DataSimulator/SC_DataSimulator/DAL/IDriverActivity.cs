using SC_DataSimulator.DomainModels;
using SC_DataSimulator.Models;

namespace SC_DataSimulator.DAL
{
    public interface IDriverActivity
    {
        public void AddDriver(string name);
        public bool ExistDriver();
        public bool ReachedMaxToday();
        public void AddActivity(Activity activity);
        public int ActivityCount();
        public List<DriverSingleViolation> DriversSingleDriveViolation();
        public DriverSingleViolation LatestDriverSingleViolation();
        public List<TotalHoursType> TotalDriveHoursTypes();
        public List<Driver> GetAllDrivers();
    }
}
