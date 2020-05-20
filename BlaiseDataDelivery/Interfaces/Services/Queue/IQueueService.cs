

namespace BlaiseDataDelivery.Interfaces.Services.Queue
{
    public interface IQueueService
    {
        void Subscribe();

        void CancelAllSubscriptions();
    }
}
