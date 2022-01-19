namespace Mango.Services.EmailApi.Models
{
    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; }

        public string CheckoutSubscription { get; set; }

        public string CheckoutTopicName { get; set; }

        public string PaymentSubscription { get; set; }

        public string PaymentTopicName { get; set; }

        public string UpdatePaymentTopicName { get; set; }

        public string EmailSubscription { get; set; }
    }
}
