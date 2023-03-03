namespace Arma.Demo.Core.Sync;
public class SyncGroup
{
    public Guid Key { get; private set; }
    public List<string> Connections { get; private set; }

    public SyncGroup(Guid key, List<string> connections)
    {
        Key = key;
        Connections = connections;
    }
}