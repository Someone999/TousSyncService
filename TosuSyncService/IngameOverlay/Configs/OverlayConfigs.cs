using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;

namespace TosuSyncService.IngameOverlay.Configs;

[SupportedOSPlatform("windows")]
public class OverlayConfigs : IOverlayConfig
{
    public List<OverlayConfigItem> OverlayConfigItems { get; set; } = new List<OverlayConfigItem>();

    private readonly MemoryMappedFile _overlayConfigsMmf =
        MemoryMappedFile.CreateOrOpen(@"Local\rtpp-overlay-configs", 65535);

    private bool _needUpdateFonts = true;

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(false); //WasChanged(Placeholder)
        bw.Write(new byte[3] { 0, 0, 0 }); //Padding
        bw.Write(OverlayConfigItems.Count);
        foreach (var item in OverlayConfigItems)
            item.WriteTo(bw);

        bw.Seek(0, SeekOrigin.Begin);
        bw.Write(_needUpdateFonts); //WasChanged
    }

    public void WriteToMmf(bool updateFonts = true)
    {
        _needUpdateFonts = updateFonts;
        this.WriteTo(_overlayConfigsMmf);
    }
}