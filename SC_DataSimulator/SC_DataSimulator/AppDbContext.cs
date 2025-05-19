using System.Data;
using Npgsql;
using SC_DataSimulator.Models;

namespace SC_DataSimulator
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext()
        {
            _connectionString = "Server=localhost;Port=5432;Database=gamma;User Id=postgres;Password=admin;";
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }

    public static class SeedData
    {
        public static List<Driver> GetInitialData()
        {
            return new List<Driver>
            {
                new Driver { Id = 1, Name = "Driver A" },
                new Driver { Id = 2, Name = "Driver B" }
            };
        }
    }
}
