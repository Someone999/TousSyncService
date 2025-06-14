namespace TosuSyncService.Mmf;

[Flags]
public enum MmfRuleset
{
    Osu = 1,
    Taiko = 2,
    CatchTheBeat = 4,
    Mania = 8,
    All = Osu | Taiko | CatchTheBeat | Mania
}