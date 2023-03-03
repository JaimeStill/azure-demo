namespace Arma.Demo.Core.Sync;
public interface ISyncService<T> : IAsyncDisposable
{
    public Task Connect(Guid key);
    public Task Join(Guid key);
    public Task Leave(Guid key);
    public Task Initialize(SyncMessage<T> message);
    public Task Notify(SyncMessage<T> message);
    public Task Complete(SyncMessage<T> message);
    public Task Return(SyncMessage<T> message);
    public Task Reject(SyncMessage<T> message);
}