using Models;

namespace Repositories
{
    public interface IUserRepository
    {
        // Task<User> FindUserByEmail(string email);
        // Task<bool> SaveChangeAsync();
        // Task CreateNewUser(User user);
        Task<User> GetUserByRefreshToken(string refreshToken);
        
    }
}