using System.Threading.Tasks;
using ScooterApp.Domain;
using ScooterApp.Domain.Auth;
using ScooterApp.Domain.Trip;

namespace ScooterApp.TripService.TripServices
{
    public interface ITripService
    {
        Task<TripDocument> CreateTrip(TripRequest tripRequest, RiderClaim riderClaims);

        Task UpdateTrip(TripDocument tripDocument);

        Task<TripDocument> GetTrip(string tripId, string scooterCode);
    }
}