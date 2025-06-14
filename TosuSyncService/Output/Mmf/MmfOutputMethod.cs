using NUnit.Framework;

namespace TosuSyncService.Output.Mmf;

public class MmfOutputMethod : IOutputMethod, IDisposable
{
    private MmfInstance _mmfInstance;

    public MmfOutputMethod(MmfInstance mmfInstance)
    {
        _mmfInstance = mmfInstance;
        _mmfInstance.PreDispose += OnPreDispose;
        _mmfInstance.Disposed += OnPreDispose;
    }

    private static readonly byte[] ZeroByteArray = new byte[16384];
    
    public bool HasData { get; private set; }
    
    private void OnPreDispose(object? sender, EventArgs e)
    {
        Clear();
        HasData = false;
    }
    
    public int Write(byte[] data)
    {
        var stream = _mmfInstance.MmfStream;
        var realWriteLength = (int) Math.Min(data.Length, _mmfInstance.Capacity);
        if(realWriteLength <= 4096)
        {
            stream.Write(data, 0, realWriteLength);
            HasData = true;
            return data.Length;
        }
        
        int writeCount = realWriteLength / 4096.0 + realWriteLength % 4096 == 0 ? 0 : 1;
        int more = realWriteLength % 4096;
        for (int i = 0; i < writeCount; i++)
        {
            stream.Write(data, i * 4096, 4096);
        }
        
        stream.Write(data, writeCount * 4096, more);
        HasData = true;
        return realWriteLength;
    }

    public void Clear()
    {
        if (!HasData)
        {
            return;
        }
        
        Rewind();
        Write(ZeroByteArray);
        HasData = false;
    }

    public void Flush()
    {
        _mmfInstance.MmfStream.Flush();
    }

    public void Rewind()
    {
        _mmfInstance.MmfStream.Seek(0, SeekOrigin.Begin);
    }

    public void Dispose()
    {
        _mmfInstance.Dispose();
    }

    public MmfInstance? ReplaceMmfInstance(MmfInstance mmfInstance, bool disposeOldMmf = true)
    {
        if (IsSameMmfInstance(mmfInstance))
        {
            return null;
        }

        if (disposeOldMmf)
        {
            _mmfInstance.Dispose();
        }
        
        _mmfInstance = mmfInstance;
        HasData = false;
        return disposeOldMmf ? null : _mmfInstance;
    }
    
    public bool IsSameMmfInstance(MmfInstance mmfInstance) => _mmfInstance.IsSameMmf(mmfInstance);
}