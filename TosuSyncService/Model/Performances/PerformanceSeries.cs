namespace TosuSyncService.Model.Performances;

public class PerformanceSeries
{
    public string Name { get; set; } = "";
    public double[] Data { get; set; } = Array.Empty<double>();
}