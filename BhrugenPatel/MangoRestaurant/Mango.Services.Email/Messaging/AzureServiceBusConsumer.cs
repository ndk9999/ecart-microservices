using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.EmailApi.Models;
using Mango.Services.EmailApi.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly ServiceBusOptions _serviceBusOptions;
        private readonly ServiceBusProcessor _paymentStatusProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IOptions<ServiceBusOptions> options)
        {
            _emailRepository = emailRepository;
            _serviceBusOptions = options.Value;

            var client = new ServiceBusClient(_serviceBusOptions.ConnectionString);
            _paymentStatusProcessor = client.CreateProcessor(_serviceBusOptions.UpdatePaymentTopicName, _serviceBusOptions.EmailSubscription);
        }

        public async Task Start()
        {
            _paymentStatusProcessor.ProcessMessageAsync += OnPaymentStatusMessageReceived;
            _paymentStatusProcessor.ProcessErrorAsync += OnErrorReceived;

            await _paymentStatusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _paymentStatusProcessor.StopProcessingAsync();
            await _paymentStatusProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task OnPaymentStatusMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var paymentResult = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            
            try
            {
                await _emailRepository.SendAndLogEmail(paymentResult);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
