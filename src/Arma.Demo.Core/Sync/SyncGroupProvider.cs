using Microsoft.AspNetCore.SignalR;

namespace Arma.Demo.Core.Sync;
public class SyncGroupProvider
{
    public SyncGroup ServiceGroup { get; private set; }
    public List<SyncGroup> SyncGroups { get; private set; }

    public SyncGroupProvider()
    {
        SyncGroups = new();
    }

    public async Task<Guid> InitializeService(Guid key, string connectionId, IGroupManager groups)
    {
        if (ServiceGroup is null)
        {
            ServiceGroup = new(key, new() { connectionId });
        }
        else
        {
            if (!ServiceGroup.Connections.Contains(connectionId))
                ServiceGroup.Connections.Add(connectionId);
        }

        await groups.AddToGroupAsync(connectionId, ServiceGroup.Key.ToString());

        return ServiceGroup.Key;
    }

    public async Task AddToGroup(Guid key, string connectionId, IGroupManager groups)
    {
        SyncGroup group = SyncGroups.FirstOrDefault(g => g.Key == key);

        if (group is null)
            SyncGroups.Add(
                new(
                    key,
                    new(){ connectionId }
                )
            );
        else
            if (!group.Connections.Contains(connectionId))
                group.Connections.Add(connectionId);
        
        await groups.AddToGroupAsync(connectionId, key.ToString());
    }

    public async Task RemoveFromGroup(Guid key, string connectionId, IGroupManager groups)
    {
        SyncGroup group = SyncGroups.FirstOrDefault(g => g.Key == key);

        if (group is not null)
        {
            if (group.Connections.Contains(connectionId))
            {
                group.Connections.Remove(connectionId);

                if (group.Connections.Count < 1)
                    SyncGroups.Remove(group);
            }
        }

        await groups.RemoveFromGroupAsync(connectionId, key.ToString());
    }
}