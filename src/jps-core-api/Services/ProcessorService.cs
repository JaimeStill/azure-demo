using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace Core.Services;
public class ProcessorService : SyncService<Package>
{
    public ProcessorService(IConfiguration config) : base(
        config.GetValue<string>("SyncServer:ProcessorUrl") ?? "https://jps-sync.azurewebsites.net/processor"
    ) { }

    public async Task<bool> SendPackage(Package package)
    {
        await EnsureConnection();

        await Join(package.Key);

        SyncMessage<Package> message = new()
        {
            Id = Guid.NewGuid(),
            Key = package.Key,
            Data = package,
            Action = SyncAction.Push,
            Message = $"Initializing Package {package.Name} for processing"
        };

        await Push(message);

        await Leave(package.Key);

        return true;
    }
}