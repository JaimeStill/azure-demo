using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;

namespace CloudCli.Clients;
public class ProcessorClient : SyncService<Package>
{
    public ProcessorClient(string endpoint) : base(endpoint)
    {
        OnPush = OnNotify = Output;
    }

    static Task Output(SyncMessage<Package> message)
    {
        Console.WriteLine(message.Message);
        return Task.CompletedTask;
    }
}