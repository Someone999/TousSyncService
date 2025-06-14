using TosuSyncService.Model.Beatmaps;
using TosuSyncService.Model.Gameplays;
using TosuSyncService.Model.Performances;
using TosuSyncService.Model.ResultScreens;
using TosuSyncService.Model.Settings;

namespace TosuSyncService.Model;

public class TosuData
{
    public TosuEnum<TosuGameState> State { get; set; } = new TosuEnum<TosuGameState>();
    public GameSession Session { get; set; } = new GameSession();
    public GameSettings Settings { get; set; } = new GameSettings();
    public UserProfile Profile { get; set; } = new UserProfile();
    public TosuBeatmap Beatmap { get; set; } = new TosuBeatmap();
    public GamePlay Play { get; set; } = new GamePlay();
    public List<LeaderboardSlot> Leaderboard { get; set; } = new List<LeaderboardSlot>();
   
    public Performance Performance { get; set; } = new Performance();
    public ResultsScreen ResultsScreen { get; set; } = new ResultsScreen();
    public Folders Folders { get; set; } = new Folders();
    public Files Files { get; set; } = new Files();
    public DirectPath DirectPath { get; set; } = new DirectPath();
}