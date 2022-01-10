namespace Mango.Services.OrderApi.Messaging
{
    public interface IServiceBusConsumer
    {
        Task Start();

        Task Stop();
    }
}
