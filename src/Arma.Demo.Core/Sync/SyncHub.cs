using Microsoft.AspNetCore.SignalR;

namespace Arma.Demo.Core.Sync;
public abstract class SyncHub<T> : Hub<ISyncHub<T>>
{
    readonly SyncService service;
    ISyncHub<T> Listeners => Clients.Group(service.Listeners.Key.ToString());

    public SyncHub(SyncServiceManager manager)
    {
        service = manager.GetService(GetType());
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        if (Context.ConnectionId == service.ConnectionId)
        {
            await Clients.Others.Disconnected();
            service.ClearConnection();
        }
        else
        {
            List<Guid> keys = service
                .SyncGroups
                .Where(x => x.Connections.Contains(Context.ConnectionId))
                .Select(x => x.Key)
                .Distinct()
                .ToList();

            foreach (Guid key in keys)
                await service.RemoveFromGroup(key, Context.ConnectionId, Groups);

            if (service.Listeners.Connections.Contains(Context.ConnectionId))
                await service.RemoveListener(Context.ConnectionId, Groups);
        }

        await base.OnDisconnectedAsync(ex);
    }

    public async Task RegisterListener()
    {
        Console.WriteLine($"Registering listener: {Context.ConnectionId}");
        await service.AddListener(Context.ConnectionId, Groups);
        Console.WriteLine($"Listener registered: {Context.ConnectionId}");
        await Clients.Caller.FinalizeListener();
    }

    public async Task RegisterService()
    {
        if (string.IsNullOrEmpty(service.ConnectionId))
        {
            Console.WriteLine($"Registering service {service.Name} at {service.Endpoint}");
            service.SetConnection(Context.ConnectionId);
            Console.WriteLine($"Service successfully registered");
            await Clients.All.Available();
        }
    }

    public async Task Join(Guid key)
    {
        Console.WriteLine($"Client {Context.ConnectionId} is joining group {key}");
        await service.AddToGroup(key, Context.ConnectionId, Groups);
    }

    public async Task Leave(Guid key)
    {
        Console.WriteLine($"Client {Context.ConnectionId} is leaving group {key}");
        await service.RemoveFromGroup(key, Context.ConnectionId, Groups);
    }

    public async Task SendPush(SyncMessage<T> message)
    {
        if (!string.IsNullOrEmpty(service.ConnectionId))
        {
            Console.WriteLine($"Initialization message received: {message.Message}");
            Console.WriteLine($"Message key: {message.Key}");

            await Clients
                .OthersInGroup(message.Key.ToString())
                .Push(message);

            Console.WriteLine($"Pushing to Service {service.Name}");
            await Clients
                .Client(service.ConnectionId).Push(message);

            await Listeners.Push(message);
        }
        else
        {
            message.Id = Guid.NewGuid();
            message.Action = SyncActionType.Notify;
            message.Message = $"Service {service.Name} is unavailable";
            
            await Clients.All.Notify(message);
        }
    }

    public async Task SendNotify(SyncMessage<T> message)
    {
        Console.WriteLine($"Notify message received: {message.Message}");
        Console.WriteLine($"Message key: {message.Key}");

        await Clients
            .OthersInGroup(message.Key.ToString())
            .Notify(message);

        await Listeners.Notify(message);
    }

    public async Task SendComplete(SyncMessage<T> message)
    {
        Console.WriteLine($"Complete message received: {message.Message}");
        Console.WriteLine($"Message key: {message.Key}");

        await Clients
            .OthersInGroup(message.Key.ToString())
            .Complete(message);

        await Listeners.Complete(message);
    }

    public async Task SendReturn(SyncMessage<T> message)
    {
        await Clients
                .OthersInGroup(message.Key.ToString())
                .Return(message);

        await Listeners.Return(message);
    }

    public async Task SendReject(SyncMessage<T> message)
    {
        await Clients
                .OthersInGroup(message.Key.ToString())
                .Reject(message);

        await Listeners.Reject(message);
    }
}