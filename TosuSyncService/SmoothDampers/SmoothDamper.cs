using TosuSyncService.Utils;

namespace TosuSyncService.SmoothDampers;

public class SmoothDamper<T> : ISmoothDamper<T> where T: IConvertible
{
    private double _velocity = 0;
    private double _lastVal;
    public double Update(T targetValue, double smoothTime, double deltaTime)
    {
        double val = targetValue is not double d ? targetValue.ToDouble(null) : d;
        _lastVal = MathUtils.SmoothDamp(_lastVal, val, ref _velocity, smoothTime, 
            double.PositiveInfinity, deltaTime);
        return _lastVal;
    }
}