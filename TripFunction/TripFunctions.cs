using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterApp.Domain;

namespace TripFunction
{
    public static class TripFunctions
    {
        [FunctionName("CreateTrip")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Create Trip triggered...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TripRequest tripRequest = JsonConvert.DeserializeObject<TripRequest>(requestBody);

            try
            {
                //TODO: Validate Trip Request if the ScooterDocument is free!

                // create a new Trip
                var trip = new Trip
                {
                    // rider information should be picked from user principle
                    Rider = new Rider { FirstName = "Ehsan", LastName = "Amini", Email = "eamini@hitachisolutions.com" },
                    Source = tripRequest.Source,
                    RequestDateTime = tripRequest.RequestDateTime,
                    Duration = 0,
                    ScooterCode = tripRequest.ScooterCode
                };

                //TODO: store Trip in Trip collection in CosmosDb

                return (ActionResult)new OkObjectResult(trip);
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