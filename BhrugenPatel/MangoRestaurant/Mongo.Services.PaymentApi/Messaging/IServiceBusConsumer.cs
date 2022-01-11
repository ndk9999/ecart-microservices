namespace Mango.Services.PaymentApi.Messaging
{
    public interface IServiceBusConsumer
    {
        Task Start();

        Task Stop();
    }
}
