using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderApi.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly ServiceBusOptions _serviceBusOptions;
        private readonly ServiceBusProcessor _checkoutProcessor;
        private readonly ServiceBusProcessor _paymentStatusProcessor;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IMessageBus messageBus, IOptions<ServiceBusOptions> options)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
            _serviceBusOptions = options.Value;

            var client = new ServiceBusClient(_serviceBusOptions.ConnectionString);
            _checkoutProcessor = client.CreateProcessor(_serviceBusOptions.CheckoutTopicName, _serviceBusOptions.CheckoutSubscription);
            _paymentStatusProcessor = client.CreateProcessor(_serviceBusOptions.UpdatePaymentTopicName, _serviceBusOptions.CheckoutSubscription);
        }

        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += OnErrorReceived;

            await _checkoutProcessor.StartProcessingAsync();

            _paymentStatusProcessor.ProcessMessageAsync += OnPaymentStatusMessageReceived;
            _paymentStatusProcessor.ProcessErrorAsync += OnErrorReceived;

            await _paymentStatusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();

            await _paymentStatusProcessor.StopProcessingAsync();
            await _paymentStatusProcessor.DisposeAsync();
        }

        private Task OnErrorReceived(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            var orderHeader = new OrderHeader
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                Phone = checkoutHeaderDto.Phone,
                CartDetails = new List<OrderDetail>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PickupDateTime = checkoutHeaderDto.PickupDateTime,
                OrderTime = DateTime.Now,
                PaymentStatus = false
            };

            foreach (var item in checkoutHeaderDto.CartDetails)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Count = item.Count
                };

                orderHeader.CartTotalItems += orderDetail.Count;
                orderHeader.CartDetails.Add(orderDetail);
            }

            await _orderRepository.AddOrder(orderHeader);

            var paymentRequestMessage = new PaymentRequestMessage
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.Id,
                OrderTotal = orderHeader.OrderTotal
            };

            try
            {
                await _messageBus.PublishMessageAsync(paymentRequestMessage, _serviceBusOptions.PaymentTopicName);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnPaymentStatusMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var paymentStatusMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentStatusMessage.OrderId, paymentStatusMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
