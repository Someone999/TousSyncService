namespace TosuSyncService.Utils;

public static class MathUtils
{
    public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
    {
        // Based on Game Programming Gems 4 Chapter 1.10
        smoothTime = Math.Max(0.0001F, smoothTime);
        double omega = 2F / smoothTime;

        double x = omega * deltaTime;
        double exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
        double change = current - target;
        double originalTo = target;

        // Clamp maximum speed
        double maxChange = maxSpeed * smoothTime;
        change = Clamp(change, -maxChange, maxChange);
        target = current - change;

        double temp = (currentVelocity + omega * change) * deltaTime;
        currentVelocity = (currentVelocity - omega * temp) * exp;
        double output = target + (change + temp) * exp;

        // Prevent overshooting
        if (originalTo - current > 0.0F == output > originalTo)
        {
            output = originalTo;
            currentVelocity = (output - originalTo) / deltaTime;
        }

        return output;
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
            value = min;
        else if (value > max)
            value = max;
        return value;
    }

    public static double GetNonNaNValue(double val, double nanReplace)
    {
        return double.IsNaN(val) ? nanReplace : val;
    }
}