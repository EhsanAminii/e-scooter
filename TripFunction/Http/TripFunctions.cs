using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterApp.Domain;
using ScooterApp.Extensions.Authentication;
using ScooterApp.Extensions.ServiceBus;
using ScooterApp.TripService.TripServices;

namespace TripFunction.Http
{
    public class TripFunctions
    {
        private readonly ITripService _tripService;

        public TripFunctions(ITripService tripService)
        {
            _tripService = tripService;
        }

        [FunctionName("CreateTrip")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [ServiceBus("TripRequested", Connection = "ScooterServiceBusConnection")] IAsyncCollector<Message> tripRequested,
            ClaimsPrincipal claimPrincipals,
            ILogger log)
        {
            log.LogInformation("Create TripDocument triggered...");

            var riderClaims = claimPrincipals.ToRiderClaims();

            if (riderClaims == null)
            {
                return new BadRequestErrorMessageResult("User is not authorized");
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tripRequest = JsonConvert.DeserializeObject<TripRequest>(requestBody);

            try
            {
                //TODO: Validate TripDocument Request if the ScooterDocument is free!

                // create a new TripDocument in Trip Collection db
                var trip = await _tripService.CreateTrip(tripRequest, riderClaims);

                // Add TripRequested event to service bus Topic
                var message = trip.ToMessage(trip.ScooterCode);
                await tripRequested.AddAsync(message);

                return new OkObjectResult(trip);
            }
            catch (Exception e)
            {
                var error = $"CreateTrip failed: {e.Message}";
                log.LogError(error);
                return new BadRequestObjectResult(error);
            }
        }
    }
}