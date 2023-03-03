using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace Core.Services;
public class ProcessorService : SyncService<Package>
{
    public ProcessorService(IConfiguration config) : base(
        config.GetValue<string>("SyncServer:ProcessorUrl") ?? "http://localhost:5100/processor"
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
            Action = SyncAction.Initialize,
            Message = $"Initializing Package {package.Name} for processing"
        };

        await Initialize(message);

        await Leave(package.Key);

        return true;
    }
}
