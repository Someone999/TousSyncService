using System.Collections.Concurrent;

namespace TosuSyncService.Output.Mmf;

public class MmfInstanceManager
{
    private MmfInstanceManager()
    {
    }
    
    private ConcurrentDictionary<string, MmfInstance> _instances = new();

    private void OnMmfDispose(object? sender, EventArgs e)
    {
        if (sender is not MmfInstance instance)
        {
            throw new ArgumentException(nameof(sender));
        }
        
        RemoveInstanceInternal(instance.MmfName);
    }
    
    public int TotalInstancesCount => _instances.Count;
    public bool TryGetMmfInstance(string name, out MmfInstance? instance)
    {
        return _instances.TryGetValue(name, out instance);
    }
    
    public MmfInstance CreateMmfInstance(string name, long capacity, bool overwriteIfExists = false)
    {
        if (_instances.TryGetValue(name, out var existing))
        {
            if (!overwriteIfExists)
            {
                throw new InvalidOperationException($"Mmf instance '{name}' already exists");
            }
            
            existing.Dispose();
        }
    
        var instance = new MmfInstance(name, capacity);
        instance.Disposed += OnMmfDispose;
        _instances[name] = instance; // 直接索引赋值更清晰
        return instance;
    }

    public MmfInstance CreateOrGetMmfInstance(string name, long capacity)
    {
        return _instances.TryGetValue(name, out var existing) 
            ? existing 
            : CreateMmfInstance(name, capacity);
    }
    
    public void RecreateMmfInstance(string name, long capacity)
    {
        if (!_instances.TryGetValue(name, out var instance))
        {
            throw new KeyNotFoundException($"Memory-mapped file instance with name '{name}' was not found.");
        }
        
        instance.Dispose();
        CreateMmfInstance(name, capacity);
    }

    public void RemoveMmfInstance(string name)
    {
        _instances.TryRemove(name, out var ins);
        if (ins == null)
        {
            return;
        }
        
        RemoveInstanceInternal(ins.MmfName);
    }
    

    public MmfInstance GetMmfInstance(string name)
    {
        if (!_instances.TryGetValue(name, out var instance))
        {
            throw new KeyNotFoundException($"Memory-mapped file instance with name '{name}' was not found.");
        }
        
        return instance;
    }

    private void RemoveInstanceInternal(string name)
    {
        _instances.TryRemove(name, out var instance);
        if (instance == null)
        {
            return;
        }
        
        instance.Disposed -= OnMmfDispose;
    }

    public void Dispose()
    {
        foreach (var instance in _instances)
        {
            instance.Value.Dispose();
        }
        
        _instances.Clear();
    }

    private static readonly Lazy<MmfInstanceManager> Lazy = new(() => new MmfInstanceManager());
    public static MmfInstanceManager Default => Lazy.Value;
}