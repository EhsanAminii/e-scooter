using System;

namespace ScooterApp.Domain
{
    public class TripRequest
    {
        public TripLocation Source { get; set; }

        public string ScooterCode { get; set; }

        public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;
    }
}