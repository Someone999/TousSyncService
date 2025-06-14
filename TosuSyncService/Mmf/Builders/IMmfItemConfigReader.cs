using HsManCommonLibrary.NestedValues;

namespace TosuSyncService.Mmf.Builders;

public interface IMmfItemConfigReader
{
    void LoadConfig(string file);
    INestedValueStore? Config { get; }
}