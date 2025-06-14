using System.IO.MemoryMappedFiles;
using TosuSyncService.Model;
using TosuSyncService.Output.Mmf;

namespace TosuSyncService.Mmf;

public interface IMmfItem : IDisposable
{
    void Init();
    string MmfName { get; set; }
    MmfState AvailableState { get; set; }
    MmfRuleset AvailableRuleset { get; set; }
    bool Enabled { get; set; }
    event MmfUpdatedEventHandler? OnMmfUpdated;
    void Update(TosuData tosuData);
    string OutputPattern { get; set; }
    MmfOutputMethod? MmfOutputMethod { get; }
}