namespace Arma.Demo.Core.Sync;
public class SyncMessage<T>
{
    // the ID for the transaction
    public Guid Id { get; set; }
    // the socket group key
    public Guid Key { get; set; }    
    public string Message { get; set; }
    public T Data { get; set; }
    public SyncActionType Action { get; set; }
}