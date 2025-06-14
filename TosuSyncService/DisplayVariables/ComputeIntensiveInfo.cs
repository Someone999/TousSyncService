using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Model;
using TosuSyncService.SmoothDampers;


namespace TosuSyncService.DisplayVariables;

#if ENABLE_COMPLEX

public class ComputeIntensiveInfo(EvaluateContext context)
{
    private GroupedVarianceCalculator _groupedVarianceCalculator = new GroupedVarianceCalculator();
    private SmoothDamper<double> _varianceSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _averageSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _maxSmoothDamper = new SmoothDamper<double>();
    private SmoothDamper<double> _minSmoothDamper = new SmoothDamper<double>();
    
    private double[]? _lastHitErrorArray;
    private void UpdateData()
    {
        var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
        if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
        {
            _groupedVarianceCalculator.Reset();
            return;
        }

        var hitErrorArr = gosuData.Play.HitErrors;
        if (hitErrorArr.Count == 0)
        {
            _groupedVarianceCalculator.Reset();
            return;
        }

        if (_lastHitErrorArray != null && _lastHitErrorArray.Length == hitErrorArr.Count)
        {
            return;
        }
        
        _lastHitErrorArray = hitErrorArr.ToArray();
        _groupedVarianceCalculator.Update(_lastHitErrorArray);
    }
    public double HitErrorVariance
    {
        get
        {
            UpdateData();
            return _varianceSmoothDamper.Update(_groupedVarianceCalculator.Variance, 33, 15);
        }
    }
    
    public double HitErrorAverage
    {
        get
        {
            UpdateData();
            return _averageSmoothDamper.Update(_groupedVarianceCalculator.Average, 33, 15);
        }
    }
    
    public double MaxHitError 
    {
        get
        {
            UpdateData();
            return _maxSmoothDamper.Update(_groupedVarianceCalculator.Max, 33, 15);
        }
    }
    
    public double MinHitError 
    {
        get
        {
            UpdateData();
            return _minSmoothDamper.Update(_groupedVarianceCalculator.Min, 33, 15);
        }
    }

}

#endif