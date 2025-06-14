using System.Runtime.Versioning;
using HsManCommonLibrary.ValueHolders;
using Newtonsoft.Json.Linq;
using osuToolsV2.Game.Legacy;
using osuToolsV2.Rulesets.Legacy;
using TosuSyncService.Model;
using TosuSyncService.Model.Settings;
using Timer = System.Timers.Timer;

namespace TosuSyncService.Mmf;

[SupportedOSPlatform("windows")]
public class MmfUpdater
{
    private ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
    private Timer _timer = new Timer();
    private List<IMmfItem> MmfItems { get; } = new List<IMmfItem>();

    private readonly object _locker = new object();
    public MmfUpdater(IValueHolder<TosuData> gosuDataHolder)
    {
        _timer.Elapsed += (_, _) =>
        {
            lock (_locker)
            {
                if (!gosuDataHolder.IsInitialized() || gosuDataHolder.Value == null)
                {
                    return;
                }
                
                UpdateAll(gosuDataHolder.Value);
            }
        };

        _timer.Interval = 50;
        _timer.AutoReset = true;
    }
    public double UpdateInterval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public void StartUpdate()
    {
        _timer.Start();
    }
    
    public void StopUpdate()
    {
        MmfItems.ForEach(f => f.MmfOutputMethod?.Clear());
        _timer.Stop();
    }
    
    
    bool PlayingModeCheck(TosuData gosuData, LegacyRuleset ruleset) => gosuData.Play.Mode.Value != ruleset;
    bool SelectSongModeCheck(TosuData gosuData, LegacyRuleset ruleset) => gosuData.Play.Mode.Value != ruleset;
    public void UpdateAll(TosuData gosuData)
    {
        _readerWriterLockSlim.EnterReadLock();
        try
        {
            foreach (var mmfItem in MmfItems)
            {
                if (mmfItem.MmfOutputMethod == null)
                {
                    continue;
                }
                
                if (!mmfItem.Enabled)
                {
                    if (mmfItem.MmfOutputMethod.HasData)
                    {
                        mmfItem.MmfOutputMethod.Clear();
                    }

                    continue;
                }

                if (mmfItem.AvailableState.GetAsGosuStates().All(state => gosuData.State != state))
                {
                    if (mmfItem.MmfOutputMethod.HasData)
                    {
                        mmfItem.MmfOutputMethod.Clear();
                    }

                    continue;
                }

                
                var isAvailableModeFunc = gosuData.State.Value switch
                {
                    TosuGameState.Play => (Func<TosuData, LegacyRuleset, bool>)PlayingModeCheck,
                    _ => (Func<TosuData, LegacyRuleset, bool>)SelectSongModeCheck
                };

                if (mmfItem.AvailableRuleset.GetAsGosuRulesets().All(ruleset => isAvailableModeFunc(gosuData, ruleset)))
                {
                    if (mmfItem.MmfOutputMethod.HasData)
                    {
                        mmfItem.MmfOutputMethod.Clear();
                    }
                    
                    continue;
                }

                bool hasCinema = (gosuData.Play.Mods.Value & LegacyGameMod.Cinema) != LegacyGameMod.None;

                if (gosuData.State == TosuGameState.Play && hasCinema)
                {
                    mmfItem.MmfOutputMethod.Clear();
                    continue;
                }
                
                mmfItem.Update(gosuData);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
       
        
       
    }

    public void AddMmf(IMmfItem mmfItem)
    {
        _readerWriterLockSlim.EnterWriteLock();
        try
        {
            mmfItem.MmfOutputMethod.Clear();
            if (MmfItems.Contains(mmfItem))
            {
                return;
            }

            mmfItem.Init();
            MmfItems.Add(mmfItem);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
        
    }
    
    public void AddFromJArray(JArray jArray)
    {
        foreach (var item in jArray)
        {
            AddFromJObject(item as JObject);
        }
    }

    public void AddFromJObject(JObject? jObject)
    {
        var mmfItem = jObject?.ToObject<ExpressionMmfItem>();
        if (mmfItem == null)
        {
            return;
        }

        AddMmf(mmfItem);
    }

    public void Clear()
    {
        _readerWriterLockSlim.EnterWriteLock();

        try
        {
            foreach (var mmf in MmfItems)
            {
                mmf.Enabled = false;
                mmf.MmfOutputMethod.Clear();
            }

            MmfItems.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
        
    }
}