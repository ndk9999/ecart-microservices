using Mango.Services.EmailApi.Contexts;
using Mango.Services.EmailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailApi.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public EmailRepository(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }


        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            await using var dbContext = new AppDbContext(_dbOptions);

            var emailLog = new EmailLog
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order #{message.OrderId} has been created successfully"
            };

            dbContext.EmailLogs.Add(emailLog);
            await dbContext.SaveChangesAsync();
        }
    }
}
