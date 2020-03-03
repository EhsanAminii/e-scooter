using System;
using System.Text;
using Bogus;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ScooterApp.Domain.Scooter;

namespace ScooterSimulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Generating scooters");
            var serviceBusConnectionString = "Endpoint=sb://scootersb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OHKNCTFtoynmxU3yzKws0nVQuZ9is7Jqb+AA/tjGv8s=";
            var topic = "scooterupdated";
            var scooterDocumentType = "Scooter";

            var client = new TopicClient(serviceBusConnectionString, topic);

            var scooterId = Guid.NewGuid().ToString();

            var scooter = new Faker<ScooterDocument>()
                .StrictMode(true)
                .RuleFor(x => x.Id, scooterId)
                .RuleFor(x => x.Location, l => new Point(l.Random.Double(10.992165, 11.150780), l.Random.Double(49.385055, 49.473032)))
                .RuleFor(x => x.Code, c => c.Random.AlphaNumeric(6))
                .RuleFor(x => x.AcceptRide, true)
                .RuleFor(x => x.Identity, scooterId)
                .RuleFor(x => x.LicensePlate, l => l.Vehicle.Vin())
                .RuleFor(x => x.DocumentType, scooterDocumentType)
                .Generate(1);

            byte[] scooterMessageBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)scooter[0]));

            var message = new Message(scooterMessageBinary)
            {
                PartitionKey = scooterId,
                ContentType = "application/json",
                SessionId = scooterId
            };

            client.SendAsync(message).Wait();
        }
    }
}