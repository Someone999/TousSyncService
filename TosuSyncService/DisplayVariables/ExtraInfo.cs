using System.Diagnostics;
using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using osuToolsV2.Beatmaps;
using osuToolsV2.Beatmaps.BreakTimes;
using osuToolsV2.Beatmaps.HitObjects;
using osuToolsV2.Game;
using osuToolsV2.Rulesets;
using osuToolsV2.Score;
using osuToolsV2.Score.ScoreProcessor;
using TosuSyncService.EventSystem;
using TosuSyncService.Mmf;
using TosuSyncService.Model;
using TosuSyncService.Model.Beatmaps;
using TosuSyncService.Model.Settings;
using TosuSyncService.PropertySynchronizer;
using TosuSyncService.Utils;
using TosuSyncService.Websockets;
using Beatmap = osuToolsV2.Beatmaps.Beatmap;

namespace TosuSyncService.DisplayVariables;

public class BreakTimeFinder
{
    private BreakTimeCollection? _breakTimeCollection;
    private bool _stopNextUpdate, _stopCurrentUpdate;
    private BreakTime? _nextBreakTime;
    private BreakTime? _currentBreakTime;
    private double _lastQueryNextOffset = -1, _lastQueryCurrentOffset = -1;

    public void ResetBreakTimes()
    {
        _currentBreakTime = null;
        _nextBreakTime = null;
    }

    public void ResetCollectionAndBreakTimes()
    {
        _breakTimeCollection = null;
        ResetBreakTimes();
    }

    public void ResetAll()
    {
        ResetCollectionAndBreakTimes();
        ResetStopStates();
    }

    public void ResetStopStates()
    {
        _stopNextUpdate = false;
        _stopCurrentUpdate = false;
    }

    public void SetBreakTimeCollection(BreakTimeCollection? breakTimeCollection)
    {
        if (breakTimeCollection == null || breakTimeCollection.Count == 0)
        {
            ResetCollectionAndBreakTimes();
            return;
        }

        _breakTimeCollection = breakTimeCollection;
        _currentBreakTime = breakTimeCollection[0];
        _nextBreakTime = null;
        SetToNextBreakTime(0);
        _stopNextUpdate = false;
        _lastQueryNextOffset = -1;
    }

    private void SetToNextBreakTime(int offset)
    {
        if (offset < _lastQueryNextOffset)
        {
            _stopNextUpdate = false;
        }

        _lastQueryNextOffset = offset;
        if (_stopNextUpdate)
        {
            return;
        }
        
        if (_breakTimeCollection == null || _breakTimeCollection.Count == 0)
        {
            _stopNextUpdate = true;
            _nextBreakTime = null;
            return;
        }
        
        var breakTime = _breakTimeCollection.SearchBreakTimeAfter(offset, false);
        if (breakTime == null || breakTime.StartTime < offset)
        {
            _stopNextUpdate = true;
            _nextBreakTime = null;
            return;
        }
        
        _nextBreakTime = breakTime;
    }
    
    
    private void SetCurrentBreakTime(double offset)
    {
        if (offset < _lastQueryCurrentOffset)
        {
            _stopCurrentUpdate = false;
            if (_breakTimeCollection == null)
            {
                return;
            }

            if (_currentBreakTime != null && _currentBreakTime.InRange(offset))
            {
                return;
            }
            
            var targetBreakTime = _breakTimeCollection.SearchBreakTimeBefore(offset, true);
            if (targetBreakTime != null && targetBreakTime.InRange(offset))
            {
                _currentBreakTime = targetBreakTime;
                return;
            }
        }
        
        _lastQueryCurrentOffset = offset;
        if (_stopCurrentUpdate)
        {
            return;
        }
        
        if (_breakTimeCollection == null || _breakTimeCollection.Count == 0)
        {
            _stopCurrentUpdate = true;
            _currentBreakTime = null;
            return;
        }

        if (_currentBreakTime != null && _currentBreakTime.InRange(offset))
        {
            return;
        }

        if (offset < _breakTimeCollection[0].StartTime)
        {
            _currentBreakTime = _breakTimeCollection[0];
            return;
        }
        
        var breakTime = _breakTimeCollection.SearchBreakTimeAfter(offset, false);
        if (breakTime == null || breakTime.EndTime <= offset)
        {
            _stopCurrentUpdate = true;
            _currentBreakTime = null;
            return;
        }
        
        _currentBreakTime = breakTime;
    }

    
    public TimeSpan GetTimeToNextBreakTime(int offset)
    {
        SetToNextBreakTime(offset);
        return _nextBreakTime == null ? TimeSpan.Zero : TimeSpan.FromMilliseconds(_nextBreakTime.StartTime - offset);
    }

    public TimeSpan GetRemainedBreakTime(int offset)
    {
        if (_breakTimeCollection == null || _breakTimeCollection.Count == 0)
        {
            return TimeSpan.Zero;
        }

        SetCurrentBreakTime(offset);
        if (_currentBreakTime == null)
        {
            return TimeSpan.Zero;
        }

        return _currentBreakTime.InRange(offset)
            ? TimeSpan.FromMilliseconds(_currentBreakTime.EndTime - offset)
            : TimeSpan.FromMilliseconds(_currentBreakTime.Duration);
    }
}

public class ExtraInfo
{
    private readonly EvaluateContext _context;

    public ExtraInfo(TosuWebSocketClient client, EvaluateContext context)
    {
        _context = context;
        var synchronizer = new GosuDataOsuToolsScoreInfoSynchronizer();
        var data = client.Data;
        var scoreInfo = new ScoreInfo();
        _osuToolsSynchronizerWrapper =
            new PropertySynchronizerWrapper<TosuData, ScoreInfo>(synchronizer, data, scoreInfo);
        var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
        _hitObjectPerTimeDetector = new HitObjectPerTimeDetector(gosuDataHolder);
        _hitObjectPerTimeDetector.Interval = 500;
    }

    private TosuBeatmap? _lastGosuBeatmap;
    private TimeSpan? _lastDuration;

    private ScoreInfo _scoreInfo = new ScoreInfo();
    private readonly PropertySynchronizerWrapper<TosuData, ScoreInfo> _osuToolsSynchronizerWrapper;
    private readonly HitObjectPerTimeDetector _hitObjectPerTimeDetector;


    public int HitObjectPerSecond => _hitObjectPerTimeDetector.CurrentValue;

    public GameGrade Grade
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return GameGrade.D;
            }

            var legacyRuleset = gosuData.Play.Mode.Value;
            var ruleset = Ruleset.FromLegacyRuleset(legacyRuleset);
            IScoreProcessor processor = ruleset.CreateScoreProcessor();
            _osuToolsSynchronizerWrapper.SynchronizeLocked();
            return processor.GetGrade(_osuToolsSynchronizerWrapper.Target);
        }
    }


    public TimeSpan CurrentTime
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return TimeSpan.Zero;
            }

            return TimeSpan.FromMilliseconds(gosuData.Beatmap.Time.CurrentTime);
        }
    }


    public TimeSpan Duration
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return TimeSpan.Zero;
            }

            var gosuBeatmap = gosuData.Beatmap;
            if (_lastGosuBeatmap != null && _lastGosuBeatmap.Checksum == gosuBeatmap.Checksum && _lastDuration != null)
            {
                return CurrentTime > _lastDuration.Value ? CurrentTime : _lastDuration.Value;
            }

            /*var beatmap = _beatmapDb.Beatmaps.FindByMd5(gosuData.GosuMenu.Beatmap.Md5);
            if (beatmap != null)
            {
                _lastDuration = beatmap.TotalTime;
                _lastGosuBeatmap = gosuBeatmap;
                return beatmap.TotalTime;
            }*/

            var currentBeatmapPath = BeatmapUtils.GetCurrentBeatmapPath(gosuData);
            if (string.IsNullOrEmpty(currentBeatmapPath))
            {
                return TimeSpan.Zero;
            }

            Beatmap? currentBeatmap = Beatmap.FromFile(currentBeatmapPath);
            var hitObjects = currentBeatmap?.HitObjects;
            if (hitObjects == null)
            {
                return TimeSpan.Zero;
            }

            var lastHitObj = hitObjects.LastOrDefault();

            var ret = lastHitObj switch
            {
                null => TimeSpan.Zero,
                IHasEndTime hasEndTime => TimeSpan.FromMilliseconds(hasEndTime.EndTime),
                _ => TimeSpan.FromMilliseconds(lastHitObj.StartTime)
            };

            _lastDuration = ret;
            _lastGosuBeatmap = gosuBeatmap;
            return ret;
        }
    }


    public string CurrentTimeStr => $"{CurrentTime.Minutes + CurrentTime.Hours * 60:d2}:{CurrentTime.Seconds:d2}";
    public string DurationStr => $"{Duration.Minutes + Duration.Hours * 60:d2}:{Duration.Seconds:d2}";
    public double TimePercent => CurrentTime / Duration;

    public string NowPlaying
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return string.Empty;
            }

            var b = gosuData.Beatmap;
            var title = b.Title;
            var artist = b.Artist;
            var difficulty = b.Version;
            return $"{artist} - {title} [{difficulty}]";
        }
    }

    public string NowPlayingUnicode
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return string.Empty;
            }

            var b = gosuData.Beatmap;
            var title = b.TitleUnicode;
            var artist = b.ArtistUnicode;
            var difficulty = b.Version;
            return $"{artist} - {title} [{difficulty}]";
        }
    }

    public int PassedHitObjectCount
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }

            var legacyRuleset = gosuData.Play.Mode.Value;
            var ruleset = Ruleset.FromLegacyRuleset(legacyRuleset);
            IScoreProcessor processor = ruleset.CreateScoreProcessor();
            _osuToolsSynchronizerWrapper.SynchronizeLocked();
            return processor.GetPassedHitObjectCount(_osuToolsSynchronizerWrapper.Target);
        }
    }

    public int HitObjectCount
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return 0;
            }

            _osuToolsSynchronizerWrapper.SynchronizeLocked();
            var ruleset = _osuToolsSynchronizerWrapper.Target.Ruleset ??
                          Ruleset.FromLegacyRuleset(gosuData.Play.Mode.Value);
            IScoreProcessor processor = ruleset.CreateScoreProcessor();
            return processor.GetHitObjectCount(_osuToolsSynchronizerWrapper.Target);
        }
    }

    public int RetryCount { get; internal set; }
    private BreakTimeFinder _finder = new BreakTimeFinder();
    private Beatmap? _currentBeatmap;

    private bool SetupBreakTimeFinder(TosuData gosuData)
    {
        var currentBeatmap = gosuData.Beatmap;
        if (_currentBeatmap != null && currentBeatmap.Checksum == _currentBeatmap.Metadata.Md5Hash)
        {
            return true;
        }
        
        var cache = Global.BeatmapMemoryCache.GetValue(gosuData.Beatmap.Checksum);
        if (cache is not Beatmap beatmap)
        {
            return false;
        }
                
        _currentBeatmap = beatmap;
        _finder.SetBreakTimeCollection(_currentBeatmap.BreakTimes);

        return true;
    }
    
    public TimeSpan RemainedBreakTime
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return TimeSpan.Zero;
            }

            if (gosuData.State != TosuGameState.Play)
            {
                return TimeSpan.Zero;
            }

            _osuToolsSynchronizerWrapper.Synchronize();
            return SetupBreakTimeFinder(gosuData) 
                ? _finder.GetRemainedBreakTime((int) gosuData.Beatmap.Time.CurrentTime) 
                : TimeSpan.Zero;


            // var beatmapBreakTimes = _currentBeatmap.BreakTimes;
            // if (beatmapBreakTimes == null || beatmapBreakTimes.Count == 0)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // var currentTime = gosuData.Beatmap.Time.CurrentTime;
            // if (currentTime < beatmapBreakTimes[0].StartTime)
            // {
            //     _currentBreakTime = beatmapBreakTimes[0];
            // }
            //
            // if (_currentBreakTime == null)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // var isCurrentBreakTime = currentTime < _currentBreakTime.EndTime;
            // if (isCurrentBreakTime && beatmapBreakTimes.InPeriod(currentTime))
            // {
            //     return TimeSpan.FromMilliseconds(_currentBreakTime.EndTime - currentTime);
            // }
            //
            // if (currentTime < _currentBreakTime.EndTime)
            // {
            //     return TimeSpan.FromMilliseconds(_currentBreakTime.Duration);
            // }
            //
            // var currentBreakTime = beatmapBreakTimes.SearchBreakTimeAfter(currentTime, true);
            // if (currentBreakTime == null)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // _currentBreakTime = currentBreakTime;
            // return TimeSpan.Zero;
        }
    }
    

    public TimeSpan TimeToNextBreakTime
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>)_context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return TimeSpan.Zero;
            }

            if (gosuData.State != TosuGameState.Play)
            {
                return TimeSpan.Zero;
            }
            
            return SetupBreakTimeFinder(gosuData) 
                ? _finder.GetTimeToNextBreakTime((int) gosuData.Beatmap.Time.CurrentTime) 
                : TimeSpan.Zero;
            

            // var cache = Global.BeatmapMemoryCache.GetValue(gosuData.Beatmap.Checksum);
            // if (cache is not Beatmap beatmap)
            // {
            //     _osuToolsSynchronizerWrapper.Synchronize();
            //     return TimeSpan.Zero;
            // }
            //
            // var beatmapBreakTimes = beatmap.BreakTimes;
            // if (beatmapBreakTimes == null || beatmapBreakTimes.Count == 0)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // var currentTime = gosuData.Beatmap.Time.CurrentTime;
            // if (currentTime < beatmapBreakTimes[0].StartTime)
            // {
            //     _currentNextBreakTime = beatmapBreakTimes[0];
            // }
            //
            // if (_currentNextBreakTime == null)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // var isNextBreakTime = currentTime < _currentNextBreakTime.StartTime;
            // if (isNextBreakTime)
            // {
            //     return TimeSpan.FromMilliseconds(_currentNextBreakTime.StartTime - currentTime);
            // }
            //
            // var currentNextBreakTime = beatmapBreakTimes.SearchBreakTimeAfter(currentTime, true);
            // if (currentNextBreakTime == null)
            // {
            //     return TimeSpan.Zero;
            // }
            //
            // _currentNextBreakTime = currentNextBreakTime;
            // return TimeSpan.Zero;
        }
    }
}