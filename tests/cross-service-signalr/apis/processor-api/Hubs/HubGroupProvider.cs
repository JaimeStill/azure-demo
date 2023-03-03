using Microsoft.AspNetCore.SignalR;

namespace Processor.Hubs;
public class HubGroupProvider
{
    public List<HubGroup> HubGroups => new();
    
    public async Task AddToGroup(Guid key, string connectionId, IGroupManager groups)
    {
        HubGroup? group = HubGroups.FirstOrDefault(g => g.Key == key);

        if (group is null)
            HubGroups.Add(
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
        HubGroup? group = HubGroups.FirstOrDefault(g => g.Key == key);

        if (group is not null)
        {
            if (group.Connections.Contains(connectionId))
            {
                group.Connections.Remove(connectionId);

                if (group.Connections.Count < 1)
                    HubGroups.Remove(group);
            }
        }

        await groups.RemoveFromGroupAsync(connectionId, key.ToString());
    }
}