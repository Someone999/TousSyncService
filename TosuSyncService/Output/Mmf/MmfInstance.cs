using System.IO.MemoryMappedFiles;

namespace TosuSyncService.Output.Mmf;

public class MmfInstance : IDisposable
{
    private MemoryMappedFile _mmf;
    private MemoryMappedViewStream _mmfStream;
    public string MmfName { get; }

    internal MmfInstance(string name, long capacity)
    {
        _mmf = MemoryMappedFile.CreateOrOpen(name, capacity);
        _mmfStream = _mmf.CreateViewStream();
        Capacity = capacity;
        MmfName = name;
    }

    public void Dispose()
    {
        PreDispose?.Invoke(this, EventArgs.Empty);
        _mmfStream.Dispose();
        _mmf.Dispose();
        Disposed?.Invoke(this, EventArgs.Empty);
        IsDisposed = true;
    }
    
    public MemoryMappedFile Mmf => _mmf;
    public Stream MmfStream => _mmfStream;
    public long Capacity { get; }
    public bool IsDisposed { get; private set; }
    public event EventHandler? Disposed;
    public event EventHandler? PreDispose;

    public bool IsSameMmf(MmfInstance mmfInstance)
    {
        var otherMmf = mmfInstance._mmf;
        var otherHandle = otherMmf.SafeMemoryMappedFileHandle.DangerousGetHandle();
        var selfHandle = _mmf.SafeMemoryMappedFileHandle.DangerousGetHandle();
        return otherHandle == selfHandle;
    }
}