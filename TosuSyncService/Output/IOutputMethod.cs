namespace TosuSyncService.Output;

public interface IOutputMethod
{
    bool HasData { get; }
    int Write(byte[] data);
    void Clear();
    void Flush();
    void Rewind();
}

