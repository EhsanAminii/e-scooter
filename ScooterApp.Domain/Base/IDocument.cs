namespace ScooterApp.Domain.Base
{
    public interface IDocument
    {
        string Id { get; set; }

        string PartitionKey { get; set; }

        string DocumentType { get; set; }
    }
}