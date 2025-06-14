using HsManCommonLibrary.Exceptions;

namespace TosuSyncService.Utils;

public static class ConvertUtils
{
    public static T? CheckedConvert<T>(object? obj)
    {
        switch (obj)
        {
            case T compatibleObj:
                return compatibleObj;
            case null:
                return default;
            case IConvertible:
                return (T?)Convert.ChangeType(obj, typeof(T));
        }
        
        throw new InvalidOperationException("Type not compatible.");
    }

    public static T? UncheckedConvert<T>(object? obj)
    {
        if (obj is IConvertible)
        {
            return (T?)Convert.ChangeType(obj, typeof(T));
        }

        return (T?)obj;
    }

    public static T NonNullCheckedConvert<T>(object? obj)
    {
        if (obj is null)
        {
            throw new NotRecoverableException(null, new ArgumentNullException(nameof(obj)));
        }
        
        var convertResult = CheckedConvert<T>(obj);

        if (convertResult is null)
        {
            throw new NotRecoverableException(null, new InvalidCastException("Can not convert to a non-null value."));
        }

        return convertResult;
    }
    
    public static T NonNullUncheckedConvert<T>(object? obj)
    {
        if (obj is null)
        {
            throw new NotRecoverableException(null, new ArgumentNullException(nameof(obj)));
        }
        
        var convertResult = UncheckedConvert<T>(obj);

        if (convertResult is null)
        {
            throw new NotRecoverableException(null, new InvalidCastException("Can not convert to a non-null value."));
        }

        return convertResult;
    }

    public static T EnsureNotNull<T>(T? obj)
    {
        if (obj is null)
        {
            throw new InvalidOperationException("Value is not nullable.");
        }

        return obj;
    }
}