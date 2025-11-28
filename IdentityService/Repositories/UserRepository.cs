using Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public UserRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // public async Task<User> FindUserByEmail(string email)
        // {
        //     var user = await _userManager.FindByEmailAsync(email);
        //     return user;
        // }

        // public async Task<bool> SaveChangeAsync()
        // {
        //     var result = await _context.SaveChangesAsync() >= 0;
        //     return result;
        // }

        // public async Task CreateNewUser(User user)
        // {
        //     if(user == null)
        //     {
        //         throw new ArgumentException(nameof(user));
        //     }
        //     await _context.AddAsync(user);
        // }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            return user;
        }
    }
}