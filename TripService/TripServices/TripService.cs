using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using ScooterApp.Domain;
using ScooterApp.Domain.Auth;
using ScooterApp.Domain.Rider;
using ScooterApp.Domain.Trip;

namespace ScooterApp.TripService.TripServices
{
    public class TripService : ITripService
    {
        private readonly CosmosClient _cosmosClient;

        public TripService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<TripDocument> CreateTrip(TripRequest tripRequest, RiderClaim riderClaims)
        {
            var tripContainer = _cosmosClient.GetContainer(Constants.ScootersDataBaseId, Constants.TripCollection);

            var trip = new TripDocument
            {
                Id = Guid.NewGuid().ToString(),
                DocumentType = Constants.TripDocumentType,
                Rider = new TripRider
                {
                    UserId = riderClaims.UserId,
                    FirstName = riderClaims.FirstName,
                    LastName = riderClaims.LastName,
                    Email = riderClaims.EmailAddress
                },
                Source = tripRequest.Source,
                RequestDateTime = tripRequest.RequestDateTime,
                Duration = 0,
                ScooterCode = tripRequest.ScooterCode,
                TripIsEnded = false,
                Destination = null,
                StartDateTime = null,
                EndDateTime = null
            };

            await tripContainer.UpsertItemAsync(trip, new PartitionKey(trip.ScooterCode));

            return trip;
        }

        public async Task UpdateTrip(TripDocument tripDocument)
        {
            var tripContainer = _cosmosClient.GetContainer(Constants.ScootersDataBaseId, Constants.TripCollection);

            await tripContainer.UpsertItemAsync(tripDocument, new PartitionKey(tripDocument.ScooterCode));
        }

        public async Task<TripDocument> GetTrip(string tripId, string scooterCode)
        {
            var tripContainer = _cosmosClient.GetContainer(Constants.ScootersDataBaseId, Constants.TripCollection);

            var result = await tripContainer.ReadItemAsync<TripDocument>(tripId, new PartitionKey(scooterCode));

            return result.Resource;
        }
    }
}