using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using ScooterApp.Domain.Scooter;

namespace ScooterApp.Service.ScooterServices
{
    public class ScooterService : IScooterService
    {
        private readonly CosmosClient _cosmosClient;

        public ScooterService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<List<ScooterDocument>> FindScootersInRange(double longitude, double latitude, int radius)
        {
            var scooterContainer = _cosmosClient.GetContainer(Constants.ScootersDataBaseId, Constants.ScooterCollection);

            var query = new QueryDefinition("SELECT c as scooter,ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@longitude, @latitude]}) as distance " +
                                            "FROM c where c.documentType = @documentType and " +
                                            "ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@longitude, @latitude]}) < @radius")

                .WithParameter("@documentType", Constants.ScooterDocumentType)
                .WithParameter("@longitude", longitude)
                .WithParameter("@latitude", latitude)
                .WithParameter("@radius", radius);

            var foundedScooters = new List<ScooterDocument>();

            var queryRequestOptions = new QueryRequestOptions()
            {
                MaxItemCount = 50
            };

            FeedIterator<JObject> scooterResultSetIterator = scooterContainer.
                GetItemQueryIterator<JObject>(query, null, queryRequestOptions);

            while (scooterResultSetIterator.HasMoreResults)
            {
                var results = await scooterResultSetIterator.ReadNextAsync();
                foreach (JObject result in results)
                {
                    var scooter = result["store"].ToObject<ScooterDocument>();
                    //scooter.Distance = result["distance"].ToObject<double>();
                    foundedScooters.Add(scooter);
                }
            }

            return foundedScooters.ToList();
        }

        public async Task UpdateScooter(ScooterDocument scooter)
        {
            var scooterContainer = _cosmosClient.GetContainer(Constants.ScootersDataBaseId, Constants.ScooterCollection);

            await scooterContainer.CreateItemAsync(scooter, new PartitionKey(scooter.PartitionKey));
        }
    }
}