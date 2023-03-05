using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace Sync.Hubs;
public class ProcessorHub : SyncHub<Package>
{
    public ProcessorHub(SyncGroupProvider groups)
        : base(groups) { }
}