using System.IO.MemoryMappedFiles;

namespace TosuSyncService.IngameOverlay;

static class Utilities
{
    public static void WriteTo(this Configs.IOverlayConfig config, MemoryMappedFile mmf)
    {
        using (var stream = mmf.CreateViewStream())
        {
            using (var bw = new BinaryWriter(stream))
                config.WriteTo(bw);
        }
    }
}