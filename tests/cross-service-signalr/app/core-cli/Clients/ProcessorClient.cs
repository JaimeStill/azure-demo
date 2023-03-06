using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace CoreCli.Clients;
public class ProcessorClient : SyncService<Package>
{
    public ProcessorClient(string endpoint) : base(endpoint)
    {
        OnPush.Set(Output);
        OnNotify.Set(Output);
    }

    static Task Output(SyncMessage<Package> message)
    {
        Console.WriteLine(message.Message);
        return Task.CompletedTask;
    }
}