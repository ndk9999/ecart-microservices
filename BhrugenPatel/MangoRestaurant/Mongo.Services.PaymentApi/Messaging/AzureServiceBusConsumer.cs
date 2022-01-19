using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.Services.PaymentApi.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.PaymentApi.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ServiceBusOptions _serviceBusOptions;
        private readonly ServiceBusProcessor _orderPaymentProcessor;
        private readonly IMessageBus _messageBus;
        private readonly IPaymentProcessor _paymentProcessor;

        public AzureServiceBusConsumer(IMessageBus messageBus, IPaymentProcessor paymentProcessor, IOptions<ServiceBusOptions> options)
        {
            _messageBus = messageBus;
            _paymentProcessor = paymentProcessor;
            _serviceBusOptions = options.Value;

            var client = new ServiceBusClient(_serviceBusOptions.ConnectionString);
            _orderPaymentProcessor = client.CreateProcessor(_serviceBusOptions.PaymentTopicName, _serviceBusOptions.PaymentSubscription);
        }

        public async Task Start()
        {
            _orderPaymentProcessor.ProcessMessageAsync += OnPaymentProcessed;
            _orderPaymentProcessor.ProcessErrorAsync += OnErrorReceived;

            await _orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _orderPaymentProcessor.StopProcessingAsync();
            await _orderPaymentProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task OnPaymentProcessed(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var paymentRequest = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var paymentResult = _paymentProcessor.ProcessPayment();

            var updatePaymentStatusMessage = new UpdatePaymentResultMessage
            {
                OrderId = paymentRequest.OrderId,
                Status = paymentResult,
                Email = paymentRequest.Email
            };

            try
            {
                await _messageBus.PublishMessageAsync(updatePaymentStatusMessage, _serviceBusOptions.UpdatePaymentTopicName);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
