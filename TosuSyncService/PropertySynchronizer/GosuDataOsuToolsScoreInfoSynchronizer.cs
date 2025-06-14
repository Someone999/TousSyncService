using System.Security.Cryptography;
using HsManCommonLibrary.PropertySynchronizer;
using HsManCommonLibrary.Utils;
using osuToolsV2.Beatmaps;
using osuToolsV2.Exceptions;
using osuToolsV2.Game.Mods;
using osuToolsV2.Rulesets;
using osuToolsV2.Rulesets.Legacy;
using osuToolsV2.Score;
using TosuSyncService.Mmf;
using TosuSyncService.Model;

namespace TosuSyncService.PropertySynchronizer;

public class GosuDataOsuToolsScoreInfoSynchronizer : IPropertySynchronizer<TosuData, ScoreInfo>
{
    private long _lastMod;
    private string _lastPath = "";
    private LegacyRuleset _lastRuleset = LegacyRuleset.None;
    private bool _usedBeatmapRuleset = false;

    private bool NeedUseBeatmapRuleset(TosuData sourceObj, ScoreInfo destinationObj, out Ruleset? beatmapRuleset)
    {
        if (destinationObj.Beatmap == null)
        {
            beatmapRuleset = null;
            return false;
        }
        
        beatmapRuleset = destinationObj.Beatmap.Ruleset;
        return beatmapRuleset.IsLegacyRuleset && beatmapRuleset.LegacyRuleset != sourceObj.Play.Mode.Value;
    }
    
    private IBeatmap? GetOrAddToCache(TosuData sourceObj, ScoreInfo destinationObj)
    {
        var beatmapFolder = sourceObj.Folders.Songs;
        var gosuBeatmapPath = sourceObj.DirectPath.BeatmapFile;
        string beatmapPath = Path.Combine(beatmapFolder, gosuBeatmapPath);

        if (_lastPath == beatmapPath)
        {
            return destinationObj.Beatmap;
        }
        
        var gosuBeatmap = sourceObj.Beatmap;
        var beatmap = Global.BeatmapMemoryCache.GetValue(gosuBeatmap.Checksum);
        if (beatmap != null)
        {
            return beatmap;
        }
        
        if (!File.Exists(beatmapPath))
        {
            return null;
        }
        
        var tmpBeatmap = Beatmap.FromFile(beatmapPath);
        if (tmpBeatmap == null)
        {
            return null;
        }
        
        _lastPath = beatmapPath;
        var md5Hash = tmpBeatmap.Metadata.Md5Hash?.ToLower()
                      ?? MD5.HashData(File.ReadAllBytes(beatmapPath)).ToHexString().ToLower();
        Global.BeatmapMemoryCache.Add(md5Hash, tmpBeatmap);
        return tmpBeatmap;
    }
    public SynchronizeResult Synchronize(TosuData sourceObj, ScoreInfo destinationObj)
    {
        var beatmap = GetOrAddToCache(sourceObj, destinationObj);
        if (beatmap != null)
        {
            destinationObj.Beatmap = beatmap;
            if (NeedUseBeatmapRuleset(sourceObj, destinationObj, out var beatmapRuleset))
            {
                _usedBeatmapRuleset = true;
                destinationObj.Ruleset = beatmapRuleset;
                _lastRuleset = beatmapRuleset?.LegacyRuleset ?? throw new InvalidBeatmapException();
            }
        }
        
        if (!_usedBeatmapRuleset && _lastRuleset != sourceObj.Play.Mode.Value)
        {
            _lastRuleset = sourceObj.Play.Mode.Value;
            destinationObj.Ruleset = Ruleset.FromLegacyRuleset(_lastRuleset);
        }

        if (_lastMod != (long)sourceObj.Play.Mods.Value)
        {
            var ruleset = destinationObj.Ruleset;
            destinationObj.Mods = ModManager.FromLegacyMods(sourceObj.Play.Mods.Value, ruleset, false);
            _lastMod = (long)sourceObj.Play.Mods.Value;
        }

        destinationObj.Score = sourceObj.Play.Score;
        destinationObj.Combo = sourceObj.Play.Combo.Current;
        destinationObj.CountGeki = sourceObj.Play.Hits.CountGeki;
        destinationObj.CountKatu = sourceObj.Play.Hits.CountKatu;
        destinationObj.Count300 = sourceObj.Play.Hits.Count300;
        destinationObj.Count100 = sourceObj.Play.Hits.Count100;
        destinationObj.Count50 = sourceObj.Play.Hits.Count50;
        destinationObj.CountMiss = sourceObj.Play.Hits.CountMiss;
        destinationObj.MaxCombo = sourceObj.Play.Combo.MaxCombo;
        destinationObj.HitErrors = sourceObj.Play.HitErrors;
        destinationObj.UnstableRate = sourceObj.Play.UnstableRate;
        destinationObj.SliderBreaks = sourceObj.Play.Hits.SliderBreaks;
        return new SynchronizeResult(SynchronizeState.Success, []);
    }

    public SynchronizeResult Synchronize(object sourceObj, object destinationObj)
    {
        if (sourceObj is not TosuData src || destinationObj is not ScoreInfo dst)
        {
            return new SynchronizeResult(SynchronizeState.Failed, []);
        }

        return Synchronize(src, dst);
    }
}