using Mango.Services.OrderApi.Contexts;
using Mango.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public OrderRepository(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var dbContext = new AppDbContext(_dbOptions);

            dbContext.OrderHeaders.Add(orderHeader);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderPaymentStatus(int orderId, bool paid)
        {
            await using var dbContext = new AppDbContext(_dbOptions);
            var orderHeader = await dbContext.OrderHeaders.FindAsync(orderId);

            if (orderHeader == null) return false;

            orderHeader.PaymentStatus = paid;
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
