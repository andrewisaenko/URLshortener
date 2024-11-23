using System.Text;
using URLshortener.Data;
using URLshortener.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


namespace URLshortener.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null || !VerifyPassword(user, password))
            {
                return null;
            }

            return user;
        }

        public async Task<User> RegisterAsync(string username, string password, int roleId)
        {
            if (await UserExistsAsync(username))
            {
                throw new InvalidOperationException("User already exists.");
            }

            var user = new User
            {
                UserName = username,
                PasswordHash = HashPassword(password),
                RoleId = roleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        private bool VerifyPassword(User user, string password)
        {            
            return user.PasswordHash == HashPassword(password);
        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {                
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool IsUserInRole(User user, string roleName)
        {
            return user.Role?.Name == roleName;
        }

        public bool CanUserPerformAction(User user, string action)
        {
            if (user.Role?.Name == "Admin")
            {
                return true; 
            }

            var allowedActions = new List<string> { "AddUrl", "DeleteOwnUrl" };
            return allowedActions.Contains(action);
        }
    }
}
