using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServMon.Models
{
    public class ServMonContextFactory : IDesignTimeDbContextFactory<ServMonContext>
    {
        public ServMonContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var optionsBuilder = new DbContextOptionsBuilder<ServMonContext>();
            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connection);
            return new ServMonContext(optionsBuilder.Options);
        }
    }
}
