using System;
using Newtonsoft.Json;
using ScooterApp.Domain.Base;
using ScooterApp.Domain.Rider;

namespace ScooterApp.Domain.Trip
{
    public class TripDocument : IDocument
    {
        public string Id { get; set; }

        public string DocumentType { get; set; }

        [JsonProperty("scooterCode")]
        public string ScooterCode { get; set; }

        [JsonProperty("rider")]
        public TripRider Rider { get; set; }

        [JsonProperty("source")]
        public TripLocation Source { get; set; }

        [JsonProperty("destination")]
        public TripLocation Destination { get; set; }

        [JsonProperty("requestDateTime")]
        public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;

        [JsonProperty("startDateTime")]
        public DateTime? StartDateTime { get; set; }

        [JsonProperty("endDateTime")]
        public DateTime? EndDateTime { get; set; }

        [JsonProperty("cancelDateTime")]
        public DateTime? CancelDateTime { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("tripIsEnded")]
        public bool TripIsEnded { get; set; }
    }
}