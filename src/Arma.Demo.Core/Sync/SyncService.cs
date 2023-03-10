using Microsoft.AspNetCore.SignalR;

namespace Arma.Demo.Core.Sync;
public class SyncService
{
    public Guid Key { get; private set; }
    public string ConnectionId { get; private set; }
    public string Name { get; private set; }
    public string Endpoint { get; private set; }
    public Type Hub { get; private set; }
    public SyncGroup Listeners { get; private set; }
    public List<SyncGroup> SyncGroups { get; private set; }

    public SyncService(string name, string endpoint, Type hub)
    {
        Key = Guid.NewGuid();
        Name = name;
        Endpoint = endpoint;
        Hub = hub;

        Listeners = new(Guid.NewGuid(), new());

        SyncGroups = new();
    }

    public async Task<bool> Ping()
    {
        try
        {
            HttpClient client = new();
            HttpResponseMessage result = await client.GetAsync(Endpoint);
            return result.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public void SetConnection(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public void ClearConnection()
    {
        ConnectionId = null;
    }

    public async Task AddListener(string connectionId, IGroupManager groups)
    {
        if (!Listeners.Connections.Contains(connectionId))
            Listeners.Connections.Add(connectionId);

        await groups.AddToGroupAsync(connectionId, Listeners.Key.ToString());
    }

    public async Task RemoveListener(string connectionId, IGroupManager groups)
    {
        if (Listeners.Connections.Contains(connectionId))
            Listeners.Connections.Remove(connectionId);

        await groups.RemoveFromGroupAsync(connectionId, Listeners.Key.ToString());
    }

    public async Task AddToGroup(Guid key, string connectionId, IGroupManager groups)
    {
        SyncGroup group = SyncGroups.FirstOrDefault(x => x.Key == key);

        if (group is null)
            SyncGroups.Add(
                new(
                    key,
                    new() { connectionId }
                )
            );
        else
            if (!group.Connections.Contains(connectionId))
            group.Connections.Add(connectionId);

        await groups.AddToGroupAsync(connectionId, key.ToString());
    }

    public async Task RemoveFromGroup(Guid key, string connectionId, IGroupManager groups)
    {
        SyncGroup group = SyncGroups.FirstOrDefault(x => x.Key == key);

        if (group is not null)
        {
            if (group.Connections.Contains(connectionId))
            {
                group.Connections.Remove(connectionId);
            }

            if (group.Connections.Count < 1)
                SyncGroups.Remove(group);
        }

        await groups.RemoveFromGroupAsync(connectionId, key.ToString());
    }
}