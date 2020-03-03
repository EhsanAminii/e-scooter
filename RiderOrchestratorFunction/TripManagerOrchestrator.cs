using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterApp.Domain.Trip;

namespace RiderOrchestratorFunction
{
    public static class TripManagerOrchestrator
    {
        [FunctionName("StartTripManager")]
        public static async Task<object> StartTripManager(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            TripDocument tripDocument = context.GetInput<TripDocument>();

            string tripAcceptCode;

            // Replace "hello" with the name of your Durable Activity Function.
            tripDocument = await context.CallActivityAsync<TripDocument>("CallForUnlockAndStart", tripDocument);

            using (var cts = new CancellationTokenSource())
            {
                var timeoutAt = context.CurrentUtcDateTime.AddSeconds(30);
                var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);
                var acknowledgeTask = context.WaitForExternalEvent<string>("RiderUnlockedAndStarted");

                var winner = await Task.WhenAny(acknowledgeTask, timeoutTask);
                if (winner == acknowledgeTask)
                {
                    tripAcceptCode = acknowledgeTask.Result;
                    cts.Cancel(); // we should cancel the timeout task
                }
                else
                {
                    tripAcceptCode = "Timed Out";
                }
            }

            return new
            {
                Trip = tripDocument,
                Code = tripAcceptCode
            };
        }

        [FunctionName("CallForUnlockAndStart")]
        public static TripDocument CallForUnlockAndStart([ActivityTrigger] TripDocument tripDocument,
            ILogger log)
        {
            log.LogInformation($"The tripDocument with code {tripDocument.ScooterCode} is waiting for unlock");

            return tripDocument;
        }

        [FunctionName("TripOrchestrator")]
        public static async Task TripOrchestrator(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequest req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TripDocument tripDocument = JsonConvert.DeserializeObject<TripDocument>(requestBody);
            var instanceId = tripDocument.ScooterCode;

            try
            {
                var tripStatus = await starter.GetStatusAsync(instanceId);
                string runningStatus = tripStatus == null ? "NULL" : tripStatus.RuntimeStatus.ToString();
                log.LogInformation($"Instance running status: '{runningStatus}'.");

                if (tripStatus == null ||
                    tripStatus.RuntimeStatus != OrchestrationRuntimeStatus.Running ||
                    tripStatus.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
                {
                    await starter.StartNewAsync("StartTripManager", instanceId, tripDocument);
                    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [FunctionName("UnlockTrip")]
        public static async Task<IActionResult> UnlockTrip(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tripmanager/{code}")] HttpRequest req,
            string code,
            [DurableClient] IDurableOrchestrationClient context,
            ILogger log)
        {
            await context.RaiseEventAsync(code, "RiderUnlockedAndStarted", code);
            return new OkObjectResult("OK");
        }
    }
}