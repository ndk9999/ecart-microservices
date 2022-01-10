using Azure.Messaging.ServiceBus;
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

        public AzureServiceBusConsumer(OrderRepository orderRepository, IOptions<ServiceBusOptions> options)
        {
            _orderRepository = orderRepository;
            _serviceBusOptions = options.Value;

            var client = new ServiceBusClient(_serviceBusOptions.ConnectionString);
            _checkoutProcessor = client.CreateProcessor(_serviceBusOptions.CheckoutTopicName, _serviceBusOptions.CheckoutSubscription);
        }

        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += OnErrorReceived;

            await _checkoutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();
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
        }
    }
}
