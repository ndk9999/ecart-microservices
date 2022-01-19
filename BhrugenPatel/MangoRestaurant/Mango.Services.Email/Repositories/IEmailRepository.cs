using Mango.Services.EmailApi.Models;

namespace Mango.Services.EmailApi.Repositories
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
