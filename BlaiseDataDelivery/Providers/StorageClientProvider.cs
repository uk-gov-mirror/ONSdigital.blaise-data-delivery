using BlaiseDataDelivery.Interfaces.Providers;
using Google.Cloud.Storage.V1;

namespace BlaiseDataDelivery.Providers
{
    public class StorageClientProvider : IStorageClientProvider
    {
        public StorageClient GetStorageClient()
        {
            return StorageClient.Create();
        }
    }
}
