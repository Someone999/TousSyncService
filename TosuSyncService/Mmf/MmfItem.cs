using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using TosuSyncService.Model;
using TosuSyncService.Output.Mmf;

namespace TosuSyncService.Mmf;

[SupportedOSPlatform("windows")]
public abstract class MmfItem(string mmfName) : IMmfItem
 {
     public abstract void Init();
     public virtual string MmfName { get; set; } = mmfName;
     public MmfState AvailableState { get; set; }
     public MmfRuleset AvailableRuleset { get; set; } = MmfRuleset.All;
     public bool Enabled { get; set; }
     public event MmfUpdatedEventHandler? OnMmfUpdated;
     public abstract void Update(TosuData tosuData);
     public string OutputPattern { get; set; } = "";
     public MmfOutputMethod? MmfOutputMethod { get; protected set; }
     public abstract void Dispose();
 }