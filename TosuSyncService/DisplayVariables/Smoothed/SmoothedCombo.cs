using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Model;
using TosuSyncService.SmoothDampers;

namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedCombo(EvaluateContext context)
{
    private SmoothDamper<double> _currentComboSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _maxComboSmoothDamper = new SmoothDamper<double>();
    public double CurrentCombo
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            return _currentComboSmoothDamper.Update(gosuData.Play.Combo.Current, 33, 15);
        }
    }
    
    public double MaxCombo
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            return _maxComboSmoothDamper.Update(gosuData.Play.Combo.MaxCombo, 33, 15);
        }
    }
}