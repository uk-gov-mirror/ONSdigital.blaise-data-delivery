

namespace BlaiseDataDelivery.Interfaces.Services
{
    public interface IPublishService
    {
        void PublishStatusUpdate(string messageType, string message);
    }
}
