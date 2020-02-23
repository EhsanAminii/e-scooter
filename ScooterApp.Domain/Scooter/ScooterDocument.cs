using Microsoft.Azure.Cosmos.Spatial;
using ScooterApp.Domain.Base;

namespace ScooterApp.Domain.Scooter
{
    public class ScooterDocument : IDocument
    {
        public string Id { get; set; }

        public string Identity { get; set; }

        /// <summary>
        /// Scooter Code : like 5Gh33S
        /// </summary>
        public string Code { get; set; }

        public string LicensePlate { get; set; }

        public bool AcceptRide { get; set; }

        public Point Location { get; set; }

        public string PartitionKey { get; set; }

        public string DocumentType { get; set; }
    }
}