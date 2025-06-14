using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Model;
using TosuSyncService.SmoothDampers;

namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedBeatmap(EvaluateContext context)
{
    private readonly SmoothDamper<double> _starSmoothDamper = new SmoothDamper<double>();
    public double StarRating
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            return _starSmoothDamper.Update(gosuData.Beatmap.Statistics.Stars.Total, 33, 15);
        }
    }
}