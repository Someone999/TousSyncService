using HsManCommonLibrary.Exceptions;
using HsManCommonLibrary.ValueHolders;
using HsManLazerConnector;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using TosuSyncService.BeatmapCaches;
using TosuSyncService.Mmf;

namespace TosuSyncService;


/// <summary>
/// Do not use this when you want to get accurate pp. <br />
/// 不要在你想要精确pp的情况下使用这个类
/// </summary>
public class LazerPerformanceCalculator
{
    private readonly ValueHolder<IWorkingBeatmap> _workingBeatmapHolder = new ValueHolder<IWorkingBeatmap>();

    public LazerPerformanceCalculator()
    {
        _workingBeatmapHolder.ValueChanged += OnWorkingBeatmapChanged;
    }

    private void OnWorkingBeatmapChanged(ValueChangedEventArgs<IWorkingBeatmap> args)
    {
       SetWorkingBeatmap(args.Value);
    }

    private void SetWorkingBeatmap(IWorkingBeatmap workingBeatmap)
    {
        if (workingBeatmap == null)
        {
            throw new HsManInternalException("Working beatmap is null.");
        }
            
        var workingBeatmapWrapper = new WorkingBeatmapWrapper(workingBeatmap);
        DifficultyCalculator = workingBeatmapWrapper.CreateDifficultyCalculator();
        PerformanceCalculator = workingBeatmapWrapper.CreatePerformanceCalculator();
        if (!Global.DifficultyAttributeCache.Contains(workingBeatmap))
        {
            Global.DifficultyAttributeCache.Add(workingBeatmap, 
                new DifficultyAttributeItem(workingBeatmapWrapper.WorkingBeatmap.BeatmapInfo.Ruleset.CreateInstance()));
        }
    }
    
    
    public IWorkingBeatmap? WorkingBeatmap
    {
        get => _workingBeatmapHolder.IsInitialized() ? _workingBeatmapHolder.Value : null;
        set
        {
            if (value == null)
            {
                PerformanceCalculator = null;
                DifficultyCalculator = null;
                return;
            }
            
            _workingBeatmapHolder.SetValue(value);
        }
    }
    
    public DifficultyCalculator? DifficultyCalculator { get; private set; }
    public PerformanceCalculator? PerformanceCalculator { get; private set;}


    private static readonly PerformanceAttributes EmptyPerformanceAttributes = new PerformanceAttributes();

    private osu.Game.Rulesets.Difficulty.DifficultyAttributes? CacheDifficulty(osu.Game.Rulesets.Mods.Mod[] mods)
    {
        if (WorkingBeatmap == null)
        {
            return null;
        }

        if (!Global.DifficultyAttributeCache.Contains(WorkingBeatmap))
        {
            Global.DifficultyAttributeCache.Add(WorkingBeatmap, 
                new DifficultyAttributeItem(WorkingBeatmap.BeatmapInfo.Ruleset.CreateInstance()));
        }

        var cache = Global.DifficultyAttributeCache.GetValue(WorkingBeatmap);
        var ruleset = WorkingBeatmap.BeatmapInfo.Ruleset.CreateInstance();
        if (cache == null)
        {
            cache = new DifficultyAttributeItem(ruleset);
            Global.DifficultyAttributeCache.Update(WorkingBeatmap, cache, null);
        }

        if (cache.DifficultyAttributesMap.TryGetValue(mods, out var difficultyAttributes))
        {
            return difficultyAttributes;
        }

        var difficultyCalculator = ruleset.CreateDifficultyCalculator(WorkingBeatmap);
        difficultyAttributes = difficultyCalculator.Calculate(mods);
        cache.DifficultyAttributesMap.Add(mods, difficultyAttributes);
        return difficultyAttributes;
    }
    
    /// <summary>
    /// Due to the algorithm in Lazer can not be applied to this method (Judgement cannot be captured accurately)  <br />
    /// This method can not provides an accurate pp. <br />
    /// 因为Lazer中的算法无法应用到这个方法(无法精确的捕获Judgement)，这个方法无法提供精确的pp。
    /// An example data set is in the compressed file, named error.html. There are 3 cases for each mode (std, taiko, catch, mania).
    /// 示例数据在压缩包中，名为error.html，为每个模式(std, taiko, catch, mania)提供三个案例
    /// </summary> 
    /// <param name="scoreInfo"></param>
    /// <returns></returns>
    public PerformanceAttributes CalculatePerformance(osu.Game.Scoring.ScoreInfo scoreInfo)
    {
        if (PerformanceCalculator == null || DifficultyCalculator == null)
        {
            return EmptyPerformanceAttributes;
        }

        var cachedAttr = CacheDifficulty(scoreInfo.Mods);
        return  cachedAttr == null
            ? EmptyPerformanceAttributes
            : PerformanceCalculator.Calculate(scoreInfo, cachedAttr);
    }
    
}