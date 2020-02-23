using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ScooterFunction.Triggers
{
    public static class ScooterUpdated
    {
        [FunctionName("ScooterUpdated")]
        public static void Run(
            [ServiceBusTrigger("ScooterUpdated", "ScooterService", Connection = "ScooterServiceBusConnection")]Message scooterUpdatedMessage,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {scooterUpdatedMessage.MessageId}");
        }
    }
}