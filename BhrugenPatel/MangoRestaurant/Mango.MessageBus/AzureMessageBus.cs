using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureMessageBus : IMessageBus
    {
        private const string connectionString = "";

        public async Task PublishMessageAsync(BaseMessage message, string topicName)
        {
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(topicName);
            var jsonData = JsonConvert.SerializeObject(message);
            var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonData))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
