using System.Collections.Generic;
using System.Threading.Tasks;
using ScooterApp.Domain.Scooter;

namespace ScooterApp.Service.ScooterServices
{
    public interface IScooterService
    {
        Task<List<ScooterDocument>> FindScootersInRange(double longitude, double latitude, int radius);

        Task UpdateScooter(ScooterDocument scooterDocument);
    }
}