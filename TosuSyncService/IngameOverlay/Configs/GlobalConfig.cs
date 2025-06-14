using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Text;

namespace TosuSyncService.IngameOverlay.Configs;

[SupportedOSPlatform("windows")]
public class GlobalConfig : IOverlayConfig
{
    
    private readonly MemoryMappedFile _globalConfigMmf =
        MemoryMappedFile.CreateOrOpen(@"Local\rtpp-overlay-global-config", 65);

    public string GlyphRanges { get; set; } = "Default"; //64b

    private readonly byte[] _glyphRangesBuffer = new byte[128];

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(false); //WasChanged(Placeholder)
        bw.Write(new byte[3] { 0, 0, 0 }); //Padding
        int size = Encoding.UTF8.GetBytes(GlyphRanges, 0, GlyphRanges.Length, _glyphRangesBuffer, 0);
        _glyphRangesBuffer[size] = 0;
        bw.Write(_glyphRangesBuffer, 0, 64);

        bw.Seek(0, SeekOrigin.Begin);
        bw.Write(true); //WasChanged
    }

    public void WriteToMmf()
    {
        this.WriteTo(_globalConfigMmf);
    }
}