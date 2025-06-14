using System.IO.Compression;
using System.Runtime.Versioning;
using System.Text;
using Newtonsoft.Json;

namespace TosuSyncService.IngameOverlay.Configs;

[SupportedOSPlatform("windows")]
class OverlayConfig
    {
        //[List(ValueList = new[] { "Default", "Chinese", "Cyrillic", "Japanese", "Korean", "Thai" })]
        public string GlyphRanges
        {
            get => Setting.GlobalConfig.GlyphRanges;
            set
            {
                Setting.GlobalConfig.GlyphRanges = value;
                Setting.GlobalConfig.WriteToMmf();
            }
        }
        
        public string OverlayConfigJson
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var gzip = new GZipStream(ms, CompressionLevel.Optimal,true))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Setting.OverlayConfigs));
                        gzip.Write(bytes,0,bytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }

            set
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(value)))
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                    using (var sr = new StreamReader(gzip))
                    {
                        Setting.OverlayConfigs = JsonConvert.DeserializeObject<OverlayConfigs>(sr.ReadToEnd()) ?? throw new Exception();
                    }
                }
                catch (Exception)
                {
                    Setting.OverlayConfigs = JsonConvert.DeserializeObject<OverlayConfigs>(value)  ?? throw new Exception();;
                }
            }
        }
        
        public string OsuExecPath
        {
            get => Setting.OsuExecPath;
            set => Setting.OsuExecPath = value;
        }
        
        public bool AcceptEula
        {
            get => Setting.AcceptEula;
            set => Setting.AcceptEula = value;
        }
        
    }
