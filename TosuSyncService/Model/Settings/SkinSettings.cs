namespace TosuSyncService.Model.Settings;

public class SkinSettings
{
    public bool UseDefaultSkinInEditor { get; set; }
    public bool IgnoreBeatmapSkins { get; set; }
    public bool TintSliderBall { get; set; }
    public bool UseTaikoSkin { get; set; }
    public string Name { get; set; } = "";
}