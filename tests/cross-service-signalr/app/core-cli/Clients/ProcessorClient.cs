using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;
using Microsoft.AspNetCore.SignalR.Client;

namespace CoreCli.Clients;
public class ProcessorClient : SyncService<Package>
{
    public ProcessorClient(string endpoint) : base(endpoint)
    {
        OnInitialize = OnNotify = Output;
    }

    static Task Output(SyncMessage<Package> message)
    {
        Console.WriteLine(message.Message);
        return Task.CompletedTask;
    }
}