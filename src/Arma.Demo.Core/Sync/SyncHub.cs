using Microsoft.AspNetCore.SignalR;

namespace Arma.Demo.Core.Sync;
public abstract class SyncHub<T> : Hub<ISyncHub<T>>
{
    readonly SyncGroupProvider groups;

    public SyncHub(SyncGroupProvider groups)
    {
        this.groups = groups;
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        List<Guid> keys = groups
            .SyncGroups
            .Where(x => x.Connections.Contains(Context.ConnectionId))
            .Select(x => x.Key)
            .Distinct()
            .ToList();

        foreach (Guid key in keys)
            await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task RegisterService(Guid key)
    {
        Console.WriteLine($"Registering service with provided key {key}");
        key = await groups.InitializeService(key, Context.ConnectionId, Groups);
        Console.WriteLine($"Service registered at {key}");
        await Clients.Caller.Registered(key);
    }

    public async Task Join(Guid key)
    {
        Console.WriteLine($"Client {Context.ConnectionId} is joining group {key}");
        await groups.AddToGroup(key, Context.ConnectionId, Groups);
    }

    public async Task Leave(Guid key)
    {
        Console.WriteLine($"Client {Context.ConnectionId} is leaving group {key}");
        await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);
    }

    public async Task SendInitialize(SyncMessage<T> message)
    {
        Console.WriteLine($"Initialization message received: {message.Message}");
        Console.WriteLine($"Message key: {message.Key}");
        Console.WriteLine($"Service Group: {groups.ServiceGroup.Key}");

        await Clients
            .Groups(
                message.Key.ToString(),
                groups.ServiceGroup.Key.ToString()
            )
            .Initialize(message);
    }

    public async Task SendNotify(SyncMessage<T> message)
    {
        Console.WriteLine($"Notify message received: {message.Message}");
        Console.WriteLine($"Message key: {message.Key}");

        await Clients
            .OthersInGroup(message.Key.ToString())
            .Notify(message);
    }

    public async Task SendComplete(SyncMessage<T> message)
    {
        Console.WriteLine($"Complete message received: {message.Message}");
        Console.WriteLine($"Message key: {message.Key}");

        await Clients
            .OthersInGroup(message.Key.ToString())
            .Complete(message);
    }

    public async Task SendReturn(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Return(message);

    public async Task SendReject(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Reject(message);
}