using SC_DataSimulator.DomainModels;
using SC_DataSimulator.Models;

namespace SC_DataSimulator.DAL
{
    public interface IUserRepository
    {
        public Task<User?> CheckUserData(UserLogIn userLogIn);
    }
}
