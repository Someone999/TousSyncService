using HsManCommonLibrary.PropertySynchronizer;
using HsManLazerConnector;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Scoring.Legacy;
using osuToolsV2.Rulesets;
using osuToolsV2.Rulesets.Legacy;
using osuToolsV2.Score.ScoreProcessor;
using TosuSyncService.Model;
using TosuSyncService.Model.Gameplays;
using ScoreInfo = osu.Game.Scoring.ScoreInfo;

namespace TosuSyncService.PropertySynchronizer;

public class GosuDataLazerScoreInfoSynchronizer : IPropertySynchronizer<TosuData, ScoreInfo>
{
    private long _lastMod;
    private string _lastPath = "";
    private LegacyRuleset _lastLegacyRuleset;
    private osu.Game.Rulesets.Ruleset? _ruleset;
    private osuToolsV2.Score.ScoreInfo _osuToolsScoreInfo = new osuToolsV2.Score.ScoreInfo();
    private IScoreProcessor? _scoreProcessor;

    private static GosuDataOsuToolsScoreInfoSynchronizer _scoreInfoSynchronizer =
        new GosuDataOsuToolsScoreInfoSynchronizer();

    private osuToolsV2.Rulesets.Ruleset? _osuToolsRuleset;
    public SynchronizeResult Synchronize(TosuData sourceObj, ScoreInfo destinationObj)
    {
        if (_lastLegacyRuleset != sourceObj.Play.Mode.Value || _ruleset == null || _osuToolsRuleset == null)
        {
            _lastLegacyRuleset = sourceObj.Play.Mode.Value;
            _ruleset = RulesetAdapter.GetLazerRuleset(sourceObj.Play.Mode.Value);
            _osuToolsRuleset = osuToolsV2.Rulesets.Ruleset.FromLegacyRuleset(_lastLegacyRuleset);
            _scoreProcessor = _osuToolsRuleset?.CreateScoreProcessor();
            destinationObj.Ruleset = _ruleset.RulesetInfo;
        }
        
        if (_lastMod != (long)sourceObj.Play.Mods.Value && _ruleset != null)
        {
            var mods = sourceObj.Play.Mods.Value;
            destinationObj.Mods = _ruleset.ConvertFromLegacyMods((LegacyMods)mods).ToArray();
            _lastMod = (int)sourceObj.Play.Mods.Value;
        }

        var beatmapFolder = sourceObj.Folders.Songs;
        var gosuBeatmapPaths = sourceObj.DirectPath;

        string beatmapPath = Path.Combine(beatmapFolder, gosuBeatmapPaths.BeatmapFile);
        if (beatmapPath != _lastPath && File.Exists(beatmapPath))
        {
            var workingBeatmap = WorkingBeatmapAdapter.OpenAsWorkingBeatmapWrapper(beatmapPath);
            destinationObj.BeatmapInfo = workingBeatmap.WorkingBeatmap.BeatmapInfo as BeatmapInfo;
            _lastPath = beatmapPath;
        }

        TosuHits hits = sourceObj.Play.Hits;
        destinationObj.SetCountGeki(hits.CountGeki);
        destinationObj.SetCountKatu(hits.CountKatu);
        destinationObj.SetCount300(hits.Count300);
        destinationObj.SetCount100(hits.Count100);
        destinationObj.SetCount50(hits.Count50);
        destinationObj.SetCountMiss(hits.CountMiss);
        destinationObj.Combo = sourceObj.Play.Combo.Current;
        destinationObj.MaxCombo = sourceObj.Play.Combo.MaxCombo;
        destinationObj.TotalScore = sourceObj.Play.Score;
        
        var r = _scoreInfoSynchronizer.Synchronize(sourceObj, _osuToolsScoreInfo);
        if (r.HasError)
        {
            return r;
        }
        
        destinationObj.Accuracy = _scoreProcessor?.GetAccuracy(_osuToolsScoreInfo) ?? 0;
        
        return new SynchronizeResult(SynchronizeState.Success, Array.Empty<FailedProperty>());
    }

    SynchronizeResult IPropertySynchronizer.Synchronize(object sourceObj, object destinationObj)
    {
        if (sourceObj is not TosuData gosuData || destinationObj is not ScoreInfo scoreInfo)
        {
            return new SynchronizeResult(SynchronizeState.Failed, Array.Empty<FailedProperty>());
        }

        return Synchronize(gosuData, scoreInfo);
    }
}