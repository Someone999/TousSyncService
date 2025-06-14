using System.Text;
using Newtonsoft.Json;
using TosuSyncService.IngameOverlay.JsonConverters;

namespace TosuSyncService.IngameOverlay.Configs;

public class OverlayConfigItem : IOverlayConfig
{
    public string Mmf { get; set; } = "rtpp-pp"; //128b

    public string FontPath { get; set; } = ""; //512b

    public int[] Position { get; set; } = new[] { 0, 0 }; //x,y 2i

    [JsonConverter(typeof(RgbaStringJsonConverter))]
    public float[] TextRgba { get; set; } = new[] { 1f, 1f, 1f, 1f }; //4f

    [JsonConverter(typeof(RgbaStringJsonConverter))]
    public float[] BackgroundRgba { get; set; } = new[] { 0.06f, 0.06f, 0.06f, 0.70f }; //4f

    [JsonConverter(typeof(RgbaStringJsonConverter))]
    public float[] BorderRgba { get; set; } = new[] { 0.43f, 0.43f, 0.5f, 0.5f }; //4f

    public float[] Pivot { get; set; } = new[] { 0f, 0f }; //2f


    public float FontSize { get; set; } = 10.0f;
    public float FontScale { get; set; } = 1.0f;

    private IList<string> _visibleStatus = new List<string>() { "Playing", "Rank" };

    public delegate void VisibilityChangedEvt(IList<string> list);

    public event VisibilityChangedEvt? VisibilityChanged;

    public bool BreakTime { get; set; } = false;

    [JsonConverter(typeof(VisibleStatusJsonConverter))]
    public IList<string> VisibleStatus
    {
        get => _visibleStatus;
        set
        {
            _visibleStatus = value;
            VisibilityChanged?.Invoke(_visibleStatus);
        }
    }

    [JsonIgnore]
    public bool Visibility { get; set; } = true;

    private readonly byte[] _stringBuffer = new byte[512];

    public void WriteTo(BinaryWriter bw)
    {
        int size = 0;

        size = Encoding.UTF8.GetBytes(Mmf, 0, Mmf.Length, _stringBuffer, 0);
        _stringBuffer[size] = 0;
        bw.Write(_stringBuffer, 0, 128);

        size = Encoding.UTF8.GetBytes(FontPath, 0, FontPath.Length, _stringBuffer, 0);
        _stringBuffer[size] = 0;
        bw.Write(_stringBuffer, 0, 512);

        bw.Write(Position[0]);
        bw.Write(Position[1]);

        WriteFloatArray(TextRgba, bw);
        WriteFloatArray(BackgroundRgba, bw);
        WriteFloatArray(BorderRgba, bw);
        WriteFloatArray(Pivot, bw);

        bw.Write((int)FontSize);
        bw.Write(FontScale);
        bw.Write(Visibility);
        bw.Write(new byte[3] { 0, 0, 0 }); //Padding
    }

    private void WriteFloatArray(float[] arr, BinaryWriter bw)
    {
        Array.ForEach(arr, item => bw.Write(item));
    }
};