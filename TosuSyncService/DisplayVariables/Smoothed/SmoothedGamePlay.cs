using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using HsManLazerConnector;
using osu.Game.Beatmaps;
using osuToolsV2.Rulesets;
using osuToolsV2.Score;
using TosuSyncService.Model;
using TosuSyncService.PropertySynchronizer;
using TosuSyncService.SmoothDampers;
using TosuSyncService.Utils;
using TosuSyncService.Websockets;

namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedGamePlay
{
    private readonly EvaluateContext _context;
    private readonly SmoothDamper<double> _count300RateSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _countGekiRateSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _timePercentSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _hitObjPercentSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _passedHitObjectCountSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _lazerPpSmoothDamper = new SmoothDamper<double>();
    private readonly SmoothDamper<double> _gekiRateRatioSmoothDamper = new SmoothDamper<double>();

    public SmoothedGamePlay(TosuWebSocketClient client, EvaluateContext context)
    {
        _context = context;
        var osuToolsSynchronizer = new GosuDataOsuToolsScoreInfoSynchronizer();
        var lazerSynchronizer = new GosuDataLazerScoreInfoSynchronizer();
        var data = client.Data;
        OsuToolsScoreSynchronizer =
            new PropertySynchronizerWrapper<TosuData, ScoreInfo>(osuToolsSynchronizer, data, new ScoreInfo());

        LazerScoreSynchronizer = new PropertySynchronizerWrapper<TosuData, osu.Game.Scoring.ScoreInfo>
            (lazerSynchronizer, data, new osu.Game.Scoring.ScoreInfo());
        Combo = new SmoothedCombo(_context);
        Hits = new SmoothedHits(context);

    }

    internal readonly PropertySynchronizerWrapper<TosuData, ScoreInfo> OsuToolsScoreSynchronizer;
    internal readonly PropertySynchronizerWrapper<TosuData, osu.Game.Scoring.ScoreInfo> LazerScoreSynchronizer;

    public SmoothedHits Hits { get; }
    private readonly SmoothDamper<double> _accuracySmoothDamper = new SmoothDamper<double>();

    public SmoothedCombo Combo { get; }
    
    public double Count300Rate
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) _context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var legacyRuleset = gosuData.Play.Mode.Value;
            var ruleset = Ruleset.FromLegacyRuleset(legacyRuleset);
            var calculator = ruleset.CreateScoreProcessor();
            OsuToolsScoreSynchronizer.Synchronize();
            var count300Rate = calculator.GetCount300Rate(OsuToolsScoreSynchronizer.Target);
            if (double.IsNaN(count300Rate))
            {
                return 0;
            }
            
            return _count300RateSmoothDamper.Update(count300Rate, 33, 15);
        }
    }
    
    public double CountGekiRate
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) _context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }

            var legacyRuleset = gosuData.Play.Mode.Value;
            var ruleset = Ruleset.FromLegacyRuleset(legacyRuleset);
            var calculator = ruleset.CreateScoreProcessor();
            OsuToolsScoreSynchronizer.Synchronize();
            var countGekiRate = calculator.GetCountGekiRate(OsuToolsScoreSynchronizer.Target);
            if (double.IsNaN(countGekiRate))
            {
                return 0;
            }
            
            return _countGekiRateSmoothDamper.Update(countGekiRate, 33, 15);
        }
    }
    
    
    public double CountGekiRatio
    {
        get
        {
            const double infinityThreshold = 1e5;
            var countGekiRate = CountGekiRate;
            if (countGekiRate == 0)
            {
                return 0;
            }

            if (Hits is { Count300: 0, CountGeki: > 0 })
            {
                return 1;
            }
            
            var val = 1.0 / countGekiRate;
            if (val > infinityThreshold)
            {
                return 0;
            }
            
            return val;
        }
    }

    public double Accuracy
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) _context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            var legacyRuleset = gosuData.Play.Mode.Value;
            var ruleset = Ruleset.FromLegacyRuleset(legacyRuleset);
            var calculator = ruleset.CreateScoreProcessor();
            OsuToolsScoreSynchronizer.Synchronize();
            var accuracy = calculator.GetAccuracy(OsuToolsScoreSynchronizer.Target);
            if (double.IsNaN(accuracy))
            {
                return 0;
            }
            
            return _accuracySmoothDamper.Update(accuracy, 33, 15);
        }
    }

    public double TimePercent
    {
        get
        {
            var extraData = (ExtraInfo) _context.SymbolTable.Symbols["extra"];
            return _timePercentSmoothDamper.Update(extraData.TimePercent, 33, 15);
        }
    }

    public double HitObjectPercent
    {
        get
        {
            var extraData = (ExtraInfo) _context.SymbolTable.Symbols["extra"];
            var percent = (double)extraData.PassedHitObjectCount / extraData.HitObjectCount;
            return _hitObjPercentSmoothDamper.Update(percent, 33, 15);
        }
    }

    public double PassedHitObjectCount
    {
        get
        {
            var extraData = (ExtraInfo) _context.SymbolTable.Symbols["extra"];
            var val = (double)extraData.PassedHitObjectCount;
            return _passedHitObjectCountSmoothDamper.Update(val, 33, 15);
        }
    }
    
    
    private LazerPerformanceCalculator _lazerPerformanceCalculator = new LazerPerformanceCalculator();
    private IWorkingBeatmap? _currentWorkingBeatmap;
    private string? _lastBeatmapPath;
    public double LazerPp
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) _context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }
            
            LazerScoreSynchronizer.Synchronize();
            var currentBeatmapPath = BeatmapUtils.GetCurrentBeatmapPath(gosuData);
            if (_lastBeatmapPath != currentBeatmapPath && !string.IsNullOrEmpty(currentBeatmapPath))
            {
                _currentWorkingBeatmap
                    = WorkingBeatmapAdapter.OpenAsWorkingBeatmapWrapper(currentBeatmapPath).WorkingBeatmap;

                _lazerPerformanceCalculator.WorkingBeatmap = _currentWorkingBeatmap;
                _lastBeatmapPath = currentBeatmapPath;
            }
            
            var ppAttr = _lazerPerformanceCalculator.CalculatePerformance(LazerScoreSynchronizer.Target);
            
            var totalPp = ppAttr.Total;
            return _lazerPpSmoothDamper.Update(MathUtils.GetNonNaNValue(totalPp, 0), 33, 15);
        }
    }
    
}