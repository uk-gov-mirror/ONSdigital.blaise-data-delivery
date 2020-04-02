
namespace BlaiseDataDelivery.Interfaces.Services.Json
{
    public interface ISerializerService
    {
        T DeserializeJsonMessage<T>(string serialisedJsonMessage);
    }
}