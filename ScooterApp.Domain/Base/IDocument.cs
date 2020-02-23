using Newtonsoft.Json;

namespace ScooterApp.Domain.Base
{
    public interface IDocument
    {
        [JsonRequired, JsonProperty("id")]
        string Id { get; set; }

        [JsonRequired, JsonProperty("documentType")]
        string DocumentType { get; set; }
    }
}