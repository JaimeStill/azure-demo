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
        IEnumerable<Guid> keys = groups
            .SyncGroups
            .Where(x => x.Connections.Contains(Context.ConnectionId))
            .Select(x => x.Key)
            .Distinct();

        foreach (Guid key in keys)
            await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task Join(Guid key) =>
        await groups.AddToGroup(key, Context.ConnectionId, Groups);

    public async Task Leave(Guid key) =>
        await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);
    
    public async Task SendInitialize(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Initialize(message);

    public async Task SendNotify(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Notify(message);

    public async Task SendComplete(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Complete(message);

    public async Task SendReturn(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Return(message);

    public async Task SendReject(SyncMessage<T> message) =>
        await Clients
            .OthersInGroup(message.Key.ToString())
            .Reject(message);
}