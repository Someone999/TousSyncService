namespace TosuSyncService.Websockets;

public interface IObjectConverter<in TSource, TTarget>
{
    TTarget? Create(TSource source);
    bool TryCreate(string source, out TTarget? result);
}