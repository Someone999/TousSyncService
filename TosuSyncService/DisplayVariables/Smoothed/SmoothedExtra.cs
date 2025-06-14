using FleeExpressionEvaluator.Evaluator.Context;
using TosuSyncService.SmoothDampers;


namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedExtra(EvaluateContext context)
{
    private SmoothDamper<double> _remainedBreakTimeSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _timeToNextBreakTimeSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _hitObjPerSecSmoothDamper = new SmoothDamper<double>();

    public double RemainedBreakTime
    {
        get
        {
            var extra = (ExtraInfo) context.SymbolTable.Symbols["extra"];
            var t = extra.RemainedBreakTime.TotalSeconds;
            return _remainedBreakTimeSmoothDamper.Update(t, 33, 15);
        }
    }
    
    public double TimeToNextBreakTime
    {
        get
        {
            var extra = (ExtraInfo) context.SymbolTable.Symbols["extra"];
            var t = extra.TimeToNextBreakTime.TotalSeconds;
            return _timeToNextBreakTimeSmoothDamper.Update(t, 33, 15);
        }
    }

    public double HitObjectPerSecond
    {
        get
        {
            var extra = (ExtraInfo) context.SymbolTable.Symbols["extra"];
            var h = extra.HitObjectPerSecond;
            return _hitObjPerSecSmoothDamper.Update(h, 33, 15);
        }
    }
}