using HsManCommonLibrary.ValueHolders;
using osuToolsV2.Rulesets;
using osuToolsV2.Score;
using TosuSyncService.Model;
using TosuSyncService.Model.Settings;
using TosuSyncService.PropertySynchronizer;
using Timer = System.Timers.Timer;

namespace TosuSyncService.DisplayVariables;

public class HitObjectPerTimeDetector
{
    private readonly System.Timers.Timer _detectTimer;
    private int _lastHitObjectCount = 0;

    private readonly GosuDataOsuToolsScoreInfoSynchronizer _gosuDataOsuToolsScoreInfoSynchronizer =
        new GosuDataOsuToolsScoreInfoSynchronizer();

    private ScoreInfo _scoreInfo = new ScoreInfo();

    public HitObjectPerTimeDetector(IValueHolder<TosuData> gosuData)
    {
        _detectTimer = new Timer();
        _detectTimer.Interval = 1000;
        
        _detectTimer.Elapsed += (_, _) =>
        {
            CurrentValue = 0;
            var data = gosuData.Value;
            if (!gosuData.IsInitialized() || data == null)
            {
                _lastHitObjectCount = 0;
                CurrentValue = 0;
                return;
            }

           

            if (data.State != TosuGameState.Play)
            {
                _lastHitObjectCount = 0;
                CurrentValue = 0;
                return;
            }
            
            if (data.Play.HealthBar.Normal <= 0 && StopWhenFailed)
            {
                return;
            }

            var scoreProcessor = Ruleset.FromLegacyRuleset(data.Play.Mode.Value).CreateScoreProcessor();
            _gosuDataOsuToolsScoreInfoSynchronizer.Synchronize(data, _scoreInfo);
            var current = scoreProcessor.GetPassedHitObjectCount(_scoreInfo);
            var differ = current - _lastHitObjectCount ;
            ValueChanged?.Invoke(this, differ);
            _lastHitObjectCount = current;
            CurrentValue = differ;
        };

        _detectTimer.AutoReset = true;
        _detectTimer.Start();
    }

    public event EventHandler<int>? ValueChanged;
    public int CurrentValue { get; private set; }

    public double Interval
    {
        get => _detectTimer.Interval;
        set => _detectTimer.Interval = value;
    }
    
   public bool StopWhenFailed { get; set; }
}