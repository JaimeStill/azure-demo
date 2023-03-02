using Microsoft.AspNetCore.SignalR;

namespace Processor.Hubs;
public class ProcessorHub : Hub
{
    readonly HubGroupProvider groups;

    public ProcessorHub(HubGroupProvider groups)
    {
        this.groups = groups;
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        IEnumerable<Guid> keys = groups
            .HubGroups
            .Where(x => x.Connections.Contains(Context.ConnectionId))
            .Select(x => x.Key)
            .Distinct();

        foreach (Guid key in keys)
            await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task TriggerJoin(Guid key)
    {
        Console.WriteLine($"{Context.ConnectionId} is joining group {key}");
        await groups.AddToGroup(key, Context.ConnectionId, Groups);
    }

    public async Task TriggerLeave(Guid key) =>
        await groups.RemoveFromGroup(key, Context.ConnectionId, Groups);

    public async Task TriggerBroadcast(Guid key, string message) =>
        await Clients
            .OthersInGroup(key.ToString())
            .SendAsync("broadcast", message);

    public async Task TriggerComplete(Guid key, string message) =>
        await Clients
            .OthersInGroup(key.ToString())
            .SendAsync("complete", message);
}