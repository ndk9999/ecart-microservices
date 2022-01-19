namespace Mango.Services.EmailApi.Messaging
{
    public interface IServiceBusConsumer
    {
        Task Start();

        Task Stop();
    }
}
