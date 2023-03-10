using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace SyncServer.Hubs;
public class ProcessorHub : SyncHub<Package>
{
    public ProcessorHub(SyncServiceManager groups)
        : base(groups) { }
}