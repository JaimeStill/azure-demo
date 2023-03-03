using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace SyncServer.Hubs;
public class ProcessorHub : SyncHub<Package>
{
    public ProcessorHub(SyncGroupProvider groups)
        : base(groups) { }
}