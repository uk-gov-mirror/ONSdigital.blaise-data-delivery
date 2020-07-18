using BlaiseDataDelivery.Interfaces.Services.Json;
using Newtonsoft.Json;

namespace BlaiseDataDelivery.Services.Json
{
    public class SerializerService : ISerializerService
    {
        public T DeserializeJsonMessage<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
