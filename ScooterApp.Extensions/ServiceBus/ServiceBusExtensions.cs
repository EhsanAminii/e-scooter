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
    }
}