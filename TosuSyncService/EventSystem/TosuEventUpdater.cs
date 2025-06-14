using HsManCommonLibrary.Utils;
using osuToolsV2.Game.Legacy;
using TosuSyncService.Model;
using TosuSyncService.Model.Beatmaps;
using TosuSyncService.Model.Gameplays;
using TosuSyncService.Model.Settings;

namespace TosuSyncService.EventSystem;

public class TosuEventUpdater
{
    public TosuEventManager EventManager { get; }

    private readonly SemaphoreSlim _raiseEventSemaphoreSlim = new SemaphoreSlim(1);
    private readonly SemaphoreSlim _raiseLeaderboardEventSemaphoreSlim = new SemaphoreSlim(1);
    private readonly SemaphoreSlim _raiseBeatmapEventSemaphoreSlim = new SemaphoreSlim(1);
    private readonly SemaphoreSlim _raiseTosuDataUpdateSemaphoreSlim = new SemaphoreSlim(1);

    private void RaiseTosuDataEvent(TosuData oldVal, TosuData nVal)
    {
        var e0 = EventManager.GetEvent<TosuEvent<TosuData>>(TosuEventKeys.TosuDataUpdated);
        var gosuDataValueChange = new ValueChange<TosuData>(oldVal, nVal);
        e0?.Raise(new TosuDataChangedEventData<TosuData>(gosuDataValueChange, gosuDataValueChange));
    }

    private async Task RaiseTosuDataEventAsync(TosuData oldVal, TosuData nVal)
    {
        await _raiseTosuDataUpdateSemaphoreSlim.WaitAsync();
        try
        {
            RaiseTosuDataEvent(oldVal, nVal);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _raiseTosuDataUpdateSemaphoreSlim.Release();
        }
    }

    private void RaiseEvent<T>(string eventKey, T oldVal, T nVal, ValueChange<TosuData> gosuDataValueChange)
    {
        if (EqualityUtils.Equals(oldVal, nVal))
        {
            return;
        }

        var e0 = EventManager.GetEvent<TosuEvent<T>>(eventKey);
        ValueChange<T> valueChange = new ValueChange<T>(oldVal, nVal);
        e0?.Raise(new TosuDataChangedEventData<T>(valueChange, gosuDataValueChange));
    }

    private async Task RaiseEventAsync<T>(string eventKey, T oldVal, T nVal, ValueChange<TosuData> gosuDataValueChange)
    {
        await _raiseEventSemaphoreSlim.WaitAsync();
        try
        {
            RaiseEvent(eventKey, oldVal, nVal, gosuDataValueChange);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _raiseEventSemaphoreSlim.Release();
        }
    }

    private LeaderboardSlot _lastChangedSlot = LeaderboardSlot.CreateLastPlayerSlot();

    private async Task RaiseLeaderboardEventAsync(LeaderboardSlot oldVal, LeaderboardSlot nVal,
        ValueChange<TosuData> gosuDataValueChange)
    {
        try
        {
            await _raiseLeaderboardEventSemaphoreSlim.WaitAsync();
            if (_lastChangedSlot.Position == nVal.Position)
            {
                return;
            }


            var e0 = EventManager.GetEvent<TosuEvent<LeaderboardSlot>>(TosuEventKeys
                .LeaderboardPositionChanged);
            var valueChange = new ValueChange<LeaderboardSlot>(_lastChangedSlot, nVal);
            e0?.Raise(new TosuDataChangedEventData<LeaderboardSlot>(valueChange, gosuDataValueChange));
            _lastChangedSlot = nVal;
        }
        catch (Exception)
        {
            //Nothing will happen
        }
        finally
        {
            _raiseLeaderboardEventSemaphoreSlim.Release();
        }
    }

    private TosuBeatmap _lastBeatmap = new TosuBeatmap();


    private void RaiseGosuBeatmapChangedEvent(TosuData oldVal, TosuData nVal)
    {
        if (_lastBeatmap.Checksum == nVal.Beatmap.Checksum)
        {
            return;
        }


        var e0 = EventManager.GetEvent<TosuEvent<TosuBeatmap>>(TosuEventKeys.BeatmapChanged);
        var nBeatmap = nVal.Beatmap;
        var valueChange = new ValueChange<TosuBeatmap>(_lastBeatmap, nBeatmap);
        var gosuDataValueChange = new ValueChange<TosuData>(oldVal, nVal);
        e0?.Raise(new TosuDataChangedEventData<TosuBeatmap>(valueChange, gosuDataValueChange));
        _lastBeatmap = nBeatmap;
    }

    private async Task RaiseGosuBeatmapChangedEventAsync(TosuData oldVal, TosuData nVal)
    {
        await _raiseBeatmapEventSemaphoreSlim.WaitAsync();
        try
        {
            RaiseGosuBeatmapChangedEvent(oldVal, nVal);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _raiseBeatmapEventSemaphoreSlim.Release();
        }
    }
    

    private bool _blockFailEvent;

    private async Task RaiseFailEvent(TosuData oldData, TosuData data)
    {
        await Task.Run(() =>
        {
            if (data.State != TosuGameState.Play)
            {
                _blockFailEvent = false;
                return;
            }

            var hp = data.Play.HealthBar.Normal;
            double hpThreshold = 199.0;
            if (hp > 0)
            {
                if (_blockFailEvent)
                {
                    _blockFailEvent = hp < hpThreshold;
                }

                return;
            }

            var currentTime = data.Beatmap.Time.CurrentTime;
            var firstHitObjTime = data.Beatmap.Time.FirstObject;
            var beforeFirstHitObj = currentTime < firstHitObjTime;
            var hits = data.Play.Hits;
            var judgementsCount = hits.CountGeki + hits.CountKatu + hits.Count300 + hits.Count100 + hits.Count50 +
                                  hits.CountMiss;

            var noJudgement = judgementsCount == 0;

            bool shouldBlock = _blockFailEvent || beforeFirstHitObj || noJudgement;
            if (shouldBlock)
            {
                return;
            }

            var mods = data.Play.Mods.Value;
            bool willNotFail = mods.HasFlag(LegacyGameMod.NoFail);

            var eventKey = willNotFail ? TosuEventKeys.NoFailHpZeroed : TosuEventKeys.Failed;
            var e = EventManager.GetEvent<TosuEvent<TosuData>>(eventKey);
            if (e == null)
            {
                return;
            }

            var gosuDataChange = new ValueChange<TosuData>(oldData, data);

            e.Raise(new TosuDataChangedEventData<TosuData>(gosuDataChange, gosuDataChange));
            _blockFailEvent = true;
        });
    }
    
    private readonly SemaphoreSlim _eventUpdateSemaphoreSlim = new SemaphoreSlim(1);

    public TosuEventUpdater(TosuEventManager eventManager)
    {
        EventManager = eventManager;
    }

    public async Task UpdateEventAsync(TosuData old, TosuData current)
    {
        try
        {
            await _eventUpdateSemaphoreSlim.WaitAsync();
            ValueChange<TosuData> valueChange = new ValueChange<TosuData>(old, current);

            #region Hits

            var oldHits = old.Play.Hits;
            var nHits = current.Play.Hits;
            await RaiseTosuDataEventAsync(old, current);
            await RaiseEventAsync(TosuEventKeys.CountGekiChanged, oldHits.CountGeki, nHits.CountGeki, valueChange);
            await RaiseEventAsync(TosuEventKeys.CountKatuChanged, oldHits.CountKatu, nHits.CountKatu, valueChange);
            await RaiseEventAsync(TosuEventKeys.Count300Changed, oldHits.Count300, nHits.Count300, valueChange);
            await RaiseEventAsync(TosuEventKeys.Count100Changed, oldHits.Count100, nHits.Count100, valueChange);
            await RaiseEventAsync(TosuEventKeys.Count50Changed, oldHits.Count50, nHits.Count50, valueChange);
            await RaiseEventAsync(TosuEventKeys.CountMissChanged, oldHits.CountMiss, nHits.CountMiss, valueChange);
            await RaiseEventAsync(TosuEventKeys.CountSliderBreakChanged, oldHits.SliderBreaks, nHits.SliderBreaks,
                valueChange);
            

            #endregion
            
            await RaiseEventAsync(TosuEventKeys.GradeChanged, old.Play.Rank.Current, current.Play.Rank.Current,
                valueChange);

            await RaiseGosuBeatmapChangedEventAsync(old, current);
            await RaiseEventAsync(TosuEventKeys.AccuracyChanged, old.Play.Accuracy, current.Play.Accuracy,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.ComboChanged, old.Play.Combo.Current,
                current.Play.Combo.Current, valueChange);
            await RaiseEventAsync(TosuEventKeys.HpChanged, old.Play.HealthBar.Normal, current.Play.HealthBar.Normal,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.NameChanged, old.Play.PlayerName, current.Play.PlayerName,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.ScoreChanged, old.Play.Score, current.Play.Score, valueChange);
            await RaiseEventAsync(TosuEventKeys.ModChanged, old.Play.Mods.Value, current.Play.Mods.Value,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.GameModeChanged, old.Play.Mode.Value, current.Play.Mode.Value,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.GameStateChanged, old.State.Value, current.State.Value,
                valueChange);
            await RaiseEventAsync(TosuEventKeys.PlayTimeChanged, old.Beatmap.Time.CurrentTime,
                current.Beatmap.Time.CurrentTime, valueChange);

            await RaiseFailEvent(old, current);
            //RaiseRetryEvent(old, current);
        }
        catch (Exception)
        {
            //Nothing will happen
        }
        finally
        {
            _eventUpdateSemaphoreSlim.Release();
        }
    }
}