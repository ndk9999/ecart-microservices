using Mango.Services.EmailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailApi.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}
