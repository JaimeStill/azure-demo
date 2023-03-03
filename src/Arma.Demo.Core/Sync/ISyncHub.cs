namespace Arma.Demo.Core.Sync;
public interface ISyncHub<T>
{
    Task Registered(Guid key);
    Task Initialize(SyncMessage<T> message);
    Task Notify(SyncMessage<T> message);
    Task Complete(SyncMessage<T> message);
    Task Return(SyncMessage<T> message);
    Task Reject(SyncMessage<T> message);
}