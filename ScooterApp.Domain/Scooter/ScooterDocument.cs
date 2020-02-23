using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;
using ScooterApp.Domain.Base;

namespace ScooterApp.Domain.Scooter
{
    public class ScooterDocument : IDocument
    {
        public string Id { get; set; }

        public string DocumentType { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Scooter Code : like 5Gh33S . This is the partitionKey
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("licensePlate")]
        public string LicensePlate { get; set; }

        [JsonProperty("acceptRide")]
        public bool AcceptRide { get; set; }

        [JsonProperty("location")]
        public Point Location { get; set; }
    }
}