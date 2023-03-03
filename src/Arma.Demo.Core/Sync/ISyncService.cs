namespace Arma.Demo.Core.Sync;
public interface ISyncService<T> : IAsyncDisposable
{
    Task EnsureConnection();
    Task Connect();
    Task Join(Guid key);
    Task Leave(Guid key);
    Task Push(SyncMessage<T> message);
    Task Notify(SyncMessage<T> message);
    Task Complete(SyncMessage<T> message);
    Task Return(SyncMessage<T> message);
    Task Reject(SyncMessage<T> message);
}