using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using ScooterApp.Domain.Trip;
using ScooterApp.Extensions.ServiceBus;
using ScooterApp.TripService.TripServices;

namespace TripOrchestratorFunction
{
    public class TripOrchestrator
    {
        private readonly ITripService _tripService;

        public TripOrchestrator(ITripService tripService)
        {
            _tripService = tripService;
        }

        [FunctionName("TripOrchestrator_HttpStart")]
        public async Task HttpStart(
            [ServiceBusTrigger("TripRequested", "TripOrchestrator", Connection = "ScooterServiceBusConnection", IsSessionsEnabled = true)]Message tripMessage,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {
                var tripDocument = tripMessage.GetContent<TripDocument>();
                var instanceId = tripDocument.Id;
                var reportStatus = await starter.GetStatusAsync(instanceId);
                string runningStatus = reportStatus == null ? "NULL" : reportStatus.RuntimeStatus.ToString();
                log.LogInformation($"Instance running status: '{runningStatus}'.");

                if (reportStatus == null || reportStatus.RuntimeStatus != OrchestrationRuntimeStatus.Running)
                {
                    await starter.StartNewAsync("ManageTrip", instanceId, tripDocument);
                    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                }
            }
            catch (Exception e)
            {
                var error = $"StartTripDemoViaQueueTrigger failed: {e.Message}";
                Console.WriteLine(e);
            }
        }

        [FunctionName("ManageTrip")]
        public async Task<object> ManageTrip(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var trip = context.GetInput<TripDocument>();

            if (!context.IsReplaying)
            {
                log.LogInformation($"Trip {trip.Id} requested.....");
            }

            var rideStatus = "Requested";

            // wait for starting the ride from rider
            using (var cts = new CancellationTokenSource())
            {
                // TODO: get timeout from Ride Settings
                var timeoutAt = context.CurrentUtcDateTime.AddSeconds(60);
                var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);
                var acknowledgeTask = context.WaitForExternalEvent<string>("RiderStartedTheRide");

                var winner = await Task.WhenAny(acknowledgeTask, timeoutTask);
                if (winner == acknowledgeTask)
                {
                    rideStatus = acknowledgeTask.Result;
                    cts.Cancel(); // we should cancel the timeout task
                }
                else
                {
                    rideStatus = "Timed Out";
                }
            }

            if (rideStatus == "Timed Out")
            {
                throw new Exception(
                    $"The Rider didn't start the ride after 60 seconds!");
            }

            trip = await context.CallActivityAsync<TripDocument>("TripOrchestrator_StartTrip", trip);

            return new { Trip = trip };
        }

        [FunctionName("TripOrchestrator_StartTrip")]
        public async Task<TripDocument> StartTrip([ActivityTrigger] TripDocument trip, ILogger log)
        {
            // read current Trip from database
            var tripDocument = await _tripService.GetTrip(trip.Id, trip.ScooterCode);

            tripDocument.StartDateTime = DateTime.UtcNow;

            // update trip
            await _tripService.UpdateTrip(tripDocument);

            log.LogInformation($"Trip {tripDocument.Id} with Scooter {tripDocument.ScooterCode} started at {tripDocument.StartDateTime}.");
            return tripDocument;
        }
    }
}