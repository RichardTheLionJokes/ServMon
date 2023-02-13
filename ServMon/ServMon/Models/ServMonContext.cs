using Microsoft.EntityFrameworkCore;

namespace ServMon.Models
{
    public class ServMonContext : DbContext
    {
        public DbSet<Server> Servers { get; set; } = null!;
        public DbSet<ServEvent> ServEvents { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public ServMonContext(DbContextOptions<ServMonContext> options)
            : base(options)
        {
        }
    }
}
