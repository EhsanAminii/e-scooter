using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScooterApp.Domain;
using ScooterApp.Service.ScooterServices;

namespace ScooterFunction
{
    public class ScooterFunctions
    {
        private readonly IScooterService _scooterService;

        public ScooterFunctions(IScooterService scooterService)
        {
            _scooterService = scooterService;
        }

        [FunctionName("FindScooters")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Start finding scooters");
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                TripLocation riderLocation = JsonConvert.DeserializeObject<TripLocation>(requestBody);

                // find all scooters around the rider location. Get data from cosmos db scooter collection
                var scooters = await _scooterService.FindScootersInRange(riderLocation.Longitude, riderLocation.Latitude, riderLocation.Distance);

                if (scooters.Any())
                {
                    return (ActionResult)new OkObjectResult(scooters);
                }

                return new NoContentResult();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new BadRequestObjectResult("An error occured!");
            }
        }
    }
}