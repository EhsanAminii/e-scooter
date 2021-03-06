﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InvoiceFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ScooterApp.Extensions.CosmosDb;

[assembly: FunctionsStartup(typeof(Startup))]

namespace InvoiceFunction
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
            //builder.Services.AddScoped<IScooterService, ScooterService>();
        }
    }
}