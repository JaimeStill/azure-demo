using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace Core.Services;
public class ProcessorConnection : SyncConnection<Package>
{
    public ProcessorConnection(IConfiguration config) : base(
        config.GetValue<string>("SyncServer:ProcessorUrl") ?? "http://localhost:5100/processor"
    ) { }

    public async Task<bool> SendPackage(Package package)
    {
        await Connect();

        await Join(package.Key);
        
        SyncMessage<Package> message = new()
        {
            Id = Guid.NewGuid(),
            Key = package.Key,
            Data = package,
            Message = $"Initializing Package {package.Name} for processing"
        };

        await Push(message);

        await Leave(package.Key);

        return true;
    }
}
