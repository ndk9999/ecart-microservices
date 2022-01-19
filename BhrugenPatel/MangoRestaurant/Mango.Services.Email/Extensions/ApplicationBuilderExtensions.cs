using Mango.Services.EmailApi.Messaging;

namespace Mango.Services.EmailApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IServiceBusConsumer>();

            var hostAppLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostAppLife.ApplicationStarted.Register(OnStart);
            hostAppLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            ServiceBusConsumer?.Start();
        }

        private static void OnStop()
        {
            ServiceBusConsumer?.Stop();
        }
    }
}
