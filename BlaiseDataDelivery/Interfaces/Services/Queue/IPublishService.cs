

namespace BlaiseDataDelivery.Interfaces.Services.Queue
{
    public interface IPublishService
    {
        void PublishStatusUpdate(string messageType, string message);
    }
}
