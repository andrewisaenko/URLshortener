using System.Threading.Tasks;
using URLshortener.Data;
using URLshortener.Models;
using System.Linq;

namespace URLshortener.Services
{
    public class RoleService
    {
        private readonly AppDbContext _appDbContext;

        public RoleService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            return await _appDbContext.Roles.FindAsync(roleId);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await Task.FromResult(_appDbContext.Roles.FirstOrDefault(r => r.Name == roleName));
        }

        public async Task CreateRoleAsync(Role role)
        {
            _appDbContext.Roles.Add(role);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            if (role != null)
            {
                _appDbContext.Roles.Remove(role);
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
