using Newtonsoft.Json;
using ScooterApp.Domain.Base;

namespace ScooterApp.Domain.Rider
{
    public class RiderDocument : IDocument
    {
        public string Id { get; set; }

        public string DocumentType { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}