using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Model;
using TosuSyncService.SmoothDampers;

namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedHits(EvaluateContext context)
{
    private readonly SmoothDamper<double> _countGekiSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _countKatuRateSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _count300SmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _count100SmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _count500SmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _countMissSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _ustableRateSmoothDamper = new SmoothDamper<double>();
   
    

    public double CountGeki
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _countGekiSmoothDamper.Update(hits.CountGeki, 33, 15);
        }
    }
    
    public double CountKatu
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _countKatuRateSmoothDamper.Update(hits.CountKatu, 33, 15);
        }
    }
    
    public double Count300
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _count300SmoothDamper.Update(hits.Count300, 33, 15);
        }
    }
    
    public double Count100
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _count100SmoothDamper.Update(hits.Count100, 33, 15);
        }
    }
    
    public double Count50
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _count500SmoothDamper.Update(hits.Count50, 33, 15);
        }
    }
    
    public double CountMiss
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play.Hits;
            return _countMissSmoothDamper.Update(hits.CountMiss, 33, 15);
        }
    }

    public double UnstableRate
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var hits = gosuData.Play;
            return _ustableRateSmoothDamper.Update(hits.UnstableRate, 33, 15);
        }
    }
    
    
}