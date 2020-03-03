using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace InvoiceFunction.Triggers
{
    public static class TripEnded
    {
        [FunctionName("TripEnded")]
        public static void Run(
            [ServiceBusTrigger("TripEnded", "InvoiceService", Connection = "ScooterServiceBusConnection")]string mySbMsg,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}