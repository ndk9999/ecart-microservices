namespace Mango.Services.OrderApi.Models
{
    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; }

        public string CheckoutSubscription { get; set; }

        public string CheckoutTopicName { get; set; }
    }
}
