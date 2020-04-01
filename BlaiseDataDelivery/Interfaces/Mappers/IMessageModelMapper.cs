using BlaiseDataDelivery.Models;

namespace BlaiseDataDelivery.Interfaces.Mappers
{
    public interface IMessageModelMapper
    {
        MessageModel MapToMessageModel(string message);
    }
}
