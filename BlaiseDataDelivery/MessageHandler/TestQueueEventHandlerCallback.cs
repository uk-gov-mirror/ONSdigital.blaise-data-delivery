using Blaise.Queue.Contracts.Interfaces.MessageHandlers;

namespace BlaiseDataDelivery.MessageHandler
{
    public class TestQueueEventHandlerCallback : IMessageHandlerCallback
    {
        public bool HandleMessage(string messageType, string message)
        {
            return false;
        }
    }
}
