using System.Collections.Generic;

namespace ScooterApp.Domain.Auth
{
    public class RiderClaim
    {
        public string UserId { get; set; }
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AreaId { get; set; }

        public string SignInName { get; set; }

        public string Salutation { get; set; }

        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}