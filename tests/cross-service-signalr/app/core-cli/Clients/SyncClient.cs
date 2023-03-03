using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace CoreCli.Clients;
public class SyncClient : SyncService<Package>
{
    public SyncClient(string endpoint) : base(endpoint) { }
}