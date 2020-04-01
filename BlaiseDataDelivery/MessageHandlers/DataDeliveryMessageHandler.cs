using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;

namespace BlaiseDataDelivery.MessageHandlers
{
    public class DataDeliveryMessageHandler : IMessageHandlerCallback
    {
        private readonly IMessageModelMapper _mapper;

        public DataDeliveryMessageHandler(IMessageModelMapper mapper)
        {
            _mapper = mapper; 
        }

        public bool HandleMessage(string messageType, string message)
        {
            return false;
        }
    }
}
