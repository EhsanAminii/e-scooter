using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace ScooterApp.Extensions.ServiceBus
{
    public static class ServiceBusExtensions
    {
        public static T GetContent<T>(this Message serviceBusMessage)
        {
            var messageBinary = serviceBusMessage.Body;

            var messageAsText = Encoding.UTF8.GetString(messageBinary);

            return JsonConvert.DeserializeObject<T>(messageAsText);
        }

        public static Message ToMessage<T>(this T data, string sessionId)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            var resultBytes = Encoding.UTF8.GetBytes(dataJson);
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = resultBytes,
                ContentType = "application/json",
                SessionId = sessionId
            };

            return message;
        }
    }
}