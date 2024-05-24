using Dapper;
using Microsoft.EntityFrameworkCore;
using SC_DataSimulator.DomainModels;
using SC_DataSimulator.Models;

namespace SC_DataSimulator.DAL
{
    public class DriverActivity : IDriverActivity
    {
        private readonly AppDbContext _dbContext;

        public DriverActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool ExistDriver()
        {
            var conn = _dbContext.CreateConnection();

            var sql = "SELECT COUNT(*) FROM tachograph.driver";

            var driversCounter = conn.ExecuteScalar<int>(sql);

            if (driversCounter > 0)
                return true;
            else
                return false;
        }

        public bool ReachedMaxToday()
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            var sql = "SELECT COUNT(*) FROM tachograph.activity WHERE \"Date\" = CURRENT_DATE;";

            var activityOfToday = conn.ExecuteScalar<int>(sql);
            conn.Close();

            if (activityOfToday >= 100) return true;
            else return false;

        }

        public void AddDriver(string name)
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "INSERT INTO tachograph.driver (\"Name\") VALUES (@name)";

            conn.Execute(sql, name);

            conn.Close();
        }

        public int ActivityCount()
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM tachograph.activity";

            var driversCount = conn.ExecuteScalar<int>(sql);
            conn.Close();

            return driversCount;
        }
 
        public List<DriverSingleViolation> DriversSingleDriveViolation()
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "SELECT EXTRACT(HOUR FROM (\"End\" - \"Start\")) AS DriveTime, \"d\".\"Id\", \"d\".\"name\" " +
                "FROM tachograph.activity ac join tachograph.driver d on \"ac\".\"DriverId\" = \"d\".\"Id\" WHERE \"ac\".\"Type\" LIKE '%Driving%' ";

            var drivers = conn.Query<DriverSingleViolation>(sql).Where(d => d.DriveTime > 3).DistinctBy(driver => driver.Name).ToList();

            conn.Close();

            return drivers;
        }
    
        public List<TotalHoursType> TotalDriveHoursTypes()
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "SELECT SUM(EXTRACT(HOUR FROM (\"End\" - \"Start\"))) AS TotalHours, \"ac\".\"Type\"" +
                "FROM tachograph.activity ac join tachograph.driver d on \"ac\".\"DriverId\" = \"d\".\"Id\" GROUP BY \"ac\".\"Type\"";

            var totalHours = conn.Query<TotalHoursType>(sql).ToList();

            conn.Close();

            return totalHours;
        }

        public async void AddActivity(Activity activity)
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "INSERT INTO tachograph.activity (\"Start\", \"End\", \"Date\", \"Type\", \"DriverId\") VALUES (@Start, @End, @Date, @Type, @DriverId)";

            await conn.ExecuteAsync(sql, activity);

            conn.Close();
        }

        public List<Driver> GetAllDrivers()
        {
            var conn = _dbContext.CreateConnection();
            conn.Open();

            string sql = "select * from tachograph.driver";

            var drivers = conn.Query<Driver>(sql).ToList();

            conn.Close();

            return drivers;
        }
    }
}
