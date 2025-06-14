using HsManCommonLibrary.PropertySynchronizer;
using HsManCommonLibrary.ValueHolders;

namespace TosuSyncService.PropertySynchronizer;

public class PropertySynchronizerWrapper<TSource, TDestination>(
    IPropertySynchronizer<TSource, TDestination> synchronizer,
    IValueHolder<TSource> source,
    TDestination target)
{
    private object _syncLocker = new object();
    public IValueHolder<TSource> Source => source;
    public TDestination Target => target;
    public SynchronizeResult Synchronize()
    {
        if (!Source.IsInitialized())
        {
            return new SynchronizeResult(SynchronizeState.Success);
        }
        
        if (Source.Value == null)
        {
            throw new InvalidOperationException("Source can not be null");
        }
        
        return synchronizer.Synchronize(Source.Value, Target);
    }

    public SynchronizeResult SynchronizeLocked()
    {
        lock (_syncLocker)
        {
            return Synchronize();
        }
    }
    
}