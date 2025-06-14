namespace TosuSyncService.SmoothDampers;

public interface IAutoSmoothDamper
{
    double Update(object ins, double smoothTime, double deltaTime);
}