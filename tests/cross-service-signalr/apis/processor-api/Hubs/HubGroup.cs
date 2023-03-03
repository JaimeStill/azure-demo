namespace Processor.Hubs;
public class HubGroup
{
    public Guid Key { get; private set; }
    public List<string> Connections { get; private set; }

    public HubGroup(Guid key, List<string> connections)
    {
        Key = key;
        Connections = connections;        
    }
}