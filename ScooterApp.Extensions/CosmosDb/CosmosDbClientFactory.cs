using System;
using Microsoft.Azure.Cosmos;

namespace ScooterApp.Extensions.CosmosDb
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly string _connectionName = "CosmosDbConnection";
        private readonly string _accountEndPoint;
        private readonly string _accountKey;

        /// <summary>
        /// default constructor, default connection string is "CosmosDbConnection"
        /// </summary>
        /// <param name="dataBaseConnectionName"></param>
        public CosmosDbClientFactory(string dataBaseConnectionName = null)
        {
            if (!string.IsNullOrEmpty(dataBaseConnectionName))
            {
                _connectionName = dataBaseConnectionName;
            }
        }

        public CosmosDbClientFactory(string accountKey, string accountEndPoint)
        {
            if (string.IsNullOrEmpty(accountKey) || string.IsNullOrEmpty(accountEndPoint))
            {
                throw new ArgumentException("The accountKey or account EndPoint is empty");
            }

            _accountEndPoint = accountEndPoint;
            _accountKey = accountKey;
        }

        public CosmosClient Build(CosmosClientOptions clientOptions = null)
        {
            if (clientOptions == null)
            {
                clientOptions = new CosmosClientOptions()
                {
                    ConnectionMode = Enum.TryParse(
                        Environment.GetEnvironmentVariable("CosmosConnectionMode"),
                        out ConnectionMode connectionMode)
                        ? connectionMode
                        : ConnectionMode.Direct
                };
            }

            if (!string.IsNullOrEmpty(_accountKey) || !string.IsNullOrEmpty(_accountEndPoint))
            {
                return new CosmosClient(_accountEndPoint, _accountKey, clientOptions);
            }

            return new CosmosClient(Environment.GetEnvironmentVariable(_connectionName), clientOptions);
        }
    }
}