using System;

namespace ScooterApp.Domain
{
    public class Trip
    {
        public string ScooterCode { get; set; }

        public Rider Rider { get; set; }

        public TripLocation Source { get; set; }

        public TripLocation Destination { get; set; }

        public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public double Duration { get; set; }
    }
}