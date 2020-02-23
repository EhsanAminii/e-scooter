using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ScooterApp.Domain.Scooter;
using ScooterApp.Extensions.ServiceBus;
using ScooterApp.Service.ScooterServices;

namespace ScooterFunction.Triggers
{
    public class ScooterUpdated
    {
        private readonly IScooterService _scooterService;

        public ScooterUpdated(IScooterService scooterService)
        {
            _scooterService = scooterService;
        }

        [FunctionName("ScooterUpdated")]
        [Singleton]
        public async Task Run(
            [ServiceBusTrigger("ScooterUpdated", "ScooterService", Connection = "ScooterServiceBusConnection", IsSessionsEnabled = true)]Message scooterUpdatedMessage,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {scooterUpdatedMessage.MessageId}");

            try
            {
                var scooterDocument = scooterUpdatedMessage.GetContent<ScooterDocument>();

                // Upsert scooter into database
                await _scooterService.UpdateScooter(scooterDocument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                log.LogError(e.Message);
                throw;
            }
        }
    }
}