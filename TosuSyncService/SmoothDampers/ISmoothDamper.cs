namespace TosuSyncService.SmoothDampers;

public interface ISmoothDamper<in T>
{
    double Update(T targetValue, double smoothTime, double deltaTime);
}