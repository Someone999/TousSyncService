namespace TosuSyncService.Model.Performances;

public class Performance
{
    public AccuracyPerformance Accuracy { get; set; } = new AccuracyPerformance();
    public PerformanceGraph Graph { get; set; } = new PerformanceGraph();
}