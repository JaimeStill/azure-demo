namespace Arma.Demo.Core.Sync;
public class SyncRegistration
{
    public Guid Key { get; set; }
    // Processor
    public string Name { get; set; }
    // https://jps-processor.azurewebsites.net/api/status
    public string Endpoint { get; set; }
    // https://jps-sync.azurewebsites.net/processor
    public string Hub { get; set; }
    public string ConnectionId { get; set; }
}