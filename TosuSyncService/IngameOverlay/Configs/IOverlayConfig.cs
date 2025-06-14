namespace TosuSyncService.IngameOverlay.Configs;

interface IOverlayConfig
{
    void WriteTo(BinaryWriter bw);
}