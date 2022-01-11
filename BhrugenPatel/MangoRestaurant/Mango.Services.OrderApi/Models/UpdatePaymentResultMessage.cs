using Mango.MessageBus;

namespace Mango.Services.OrderApi.Models
{
    public class UpdatePaymentResultMessage : BaseMessage
    {
        public int OrderId { get; set; }

        public bool Status { get; set; }
    }
}
