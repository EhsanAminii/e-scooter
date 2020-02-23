using Microsoft.Azure.Cosmos;

namespace ScooterApp.Extensions.CosmosDb
{
    public interface ICosmosDbClientFactory
    {
        CosmosClient Build(CosmosClientOptions clientOptions = null);
    }
}