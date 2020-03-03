using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace RiderOrchestratorFunction
{
    public static class TripManagerOrchestratorTrigger
    {
        [FunctionName("StartTrip")]
        public static async Task Run(
            [ServiceBusTrigger("%TripRiderQueue%", Connection = "scooter.servicebusqueue.connection")]Message message,
            [DurableClient]IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.MessageId}");
        }
    }
}