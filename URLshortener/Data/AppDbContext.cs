using Microsoft.EntityFrameworkCore;
using URLshortener.Models;

namespace URLshortener.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        

    }
}
