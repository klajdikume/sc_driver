using Dapper;
using SC_DataSimulator.DomainModels;
using SC_DataSimulator.Models;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace SC_DataSimulator.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> CheckUserData(UserLogIn userLogIn)
        {
            // var hashedPassword = ComputeSha256Hash(userLogIn.Password);

            var conn = _dbContext.CreateConnection();

            var query = "SELECT * FROM tachograph.\"user\" WHERE \"Name\" = @Name AND \"Password\" = @Password";
            
            var user = await conn.QueryFirstOrDefaultAsync<User>(query, new { userLogIn.Name, userLogIn.Password });

            conn.Close();

            return user;
        }

        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
