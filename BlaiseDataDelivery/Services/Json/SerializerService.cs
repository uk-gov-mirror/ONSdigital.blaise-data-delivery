using BlaiseDataDelivery.Interfaces.Services.Json;
using Newtonsoft.Json;

namespace BlaiseDataDelivery.Services.Json
{
    public class SerializerService : ISerializerService
    {
        public T DeserializeJsonMessage<T>(string serialisedJsonMessage)
        {
            return JsonConvert.DeserializeObject<T>(serialisedJsonMessage);
        }
    }
}
