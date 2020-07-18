
namespace BlaiseDataDelivery.Interfaces.Providers
{
    public interface IConfigurationProvider
    {
        string ProjectId { get; }

        string SubscriptionTopicId { get; }

        string SubscriptionId { get; }

        string VmName { get; }

        string FilePattern { get; }

        string BucketName { get; }

        string CloudStorageKey { get; }

        string EncryptionKey { get; }
    }
}
