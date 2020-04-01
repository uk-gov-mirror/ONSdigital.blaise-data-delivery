
namespace BlaiseDataDelivery.Interfaces.Services
{
    public interface IInitialiseDeliveryService
    {
        void SetupSubscription();

        void CancelSubscription();
    }
}
