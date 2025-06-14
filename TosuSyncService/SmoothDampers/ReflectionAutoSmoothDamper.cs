using System.Reflection;
using TosuSyncService.Utils;

namespace TosuSyncService.SmoothDampers;

public class ReflectionAutoSmoothDamper(MemberInfo memberInfo) : IAutoSmoothDamper
{
    private double _velocity;
    private double _lastValue;
    private bool? _isCompatibleType;

    private bool IsSubTypeOf<TParent>(Type t)
    {
        if (_isCompatibleType.HasValue)
        {
            return _isCompatibleType.Value;
        }
        
        Type parentType = typeof(TParent);
        if (parentType.IsInterface)
        {
            var implementedInterface = t.GetInterface(parentType.FullName ?? parentType.Name) != null;
            _isCompatibleType = implementedInterface;
            return _isCompatibleType.Value;
        }

        var derivesFromType = t.IsSubclassOf(parentType);
        _isCompatibleType = derivesFromType;
        return _isCompatibleType.Value;
    }
    public double Update(object ins, double smoothTime, double deltaTime)
    {
        object? val = null;
        switch (memberInfo.MemberType) 
        {
            case MemberTypes.Field:
                var fieldInfo = (FieldInfo)memberInfo;
                val = fieldInfo.GetValue(ins);
                break;
            
            case MemberTypes.Property:
                var propertyInfo = (PropertyInfo)memberInfo;
                val = propertyInfo.GetValue(ins);
                break;
            
            case MemberTypes.Method:
                var methodInfo = (MethodInfo)memberInfo;
                val = methodInfo.Invoke(ins, Array.Empty<object?>());
                break;
            default : throw new NotSupportedException();
        }

        if (val is not IConvertible convertible)
        {
            throw new NotSupportedException();
        }

        _lastValue = MathUtils.SmoothDamp(_lastValue, convertible.ToDouble(null), ref _velocity, smoothTime,
            double.PositiveInfinity, deltaTime);
        return _lastValue;
    }
}