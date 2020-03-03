using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ScooterApp.Extensions.CosmosDb;
using ScooterApp.TripService.TripServices;
using TripFunction;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TripFunction
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>() { new StringEnumConverter() }
            };

            builder.Services.AddSingleton((s) => new CosmosDbClientFactory("ScooterAppDatabaseConnection").Build());
            builder.Services.AddScoped<ITripService, TripService>();
        }
    }
}