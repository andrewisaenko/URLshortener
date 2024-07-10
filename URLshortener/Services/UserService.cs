using URLshortener.Data;
using URLshortener.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URLshortener.Services
{
    public class UserService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _appDbContext.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByNameAsync(string username)
        {
            return await Task.FromResult(_appDbContext.Users.FirstOrDefault(u => u.UserName == username));
        }

        public async Task CreateUserAsync(User user)
        {
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                _appDbContext.Users.Remove(user);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public int GetCurrentUserId()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            var user = _appDbContext.Users.FirstOrDefault(u => u.UserName == username);
            return user?.Id ?? 0;
        }

    }
}
