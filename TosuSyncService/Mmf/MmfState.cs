namespace TosuSyncService.Mmf;

[Flags]
public enum MmfState
{
    Menu = 0,
    Edit = 1,
    Play = 2,
    Exit = 4,
    SelectEdit = 8,
    SelectPlay = 16,
    SelectDrawings = 32,
    ResultScreen = 64,
    Update = 128,
    Busy = 256,
    Unknown = 512,
    Lobby = 1024,
    MatchSetup = 2048,
    SelectMulti = 4096,
    RankingVs = 8192,
    OnlineSelection = 16384,
    OptionsOffsetWizard = 32768,
    RankingTagCoop = 65536,
    RankingTeam = 131072,
    BeatmapImport = 262144,
    PackageUpdater = 524288,
    Benchmark = 1048576,
    Tourney = 2097152,
    Charts = 4194304
}