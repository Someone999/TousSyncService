using System.Runtime.Versioning;
using HsManCommonLibrary.ValueHolders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TosuSyncService.IngameOverlay.Configs;
using TosuSyncService.Mmf.Builders;
using TosuSyncService.Mmf.FileWatchers;
using TosuSyncService.Model;
using Timer = System.Timers.Timer;

namespace TosuSyncService.Mmf;

[SupportedOSPlatform("windows")]
public class MmfManager
{
    public IValueHolder<TosuData> TosuDataHolder { get; }
    public MmfUpdater Updater { get; private set; }
    public IMmfItem[] MmfItems { get; private set; } = [];
    private const string OverlayMmfConfigFile = @"configs\overlayMmfConfig.json";
    private const string MmfConfigFile = @"configs\mmfConfig.json";
    private FileWriteWatcher _mmfConfigWriteWatcher = new FileWriteWatcher(MmfConfigFile);
    private FileWriteWatcher _overlayConfigWriteWatcher = new FileWriteWatcher(OverlayMmfConfigFile);

    private readonly object _locker = new object();
    private readonly Timer _configFileUpdateTimer = new Timer();
    
    private MmfManager(IValueHolder<TosuData> tosuDataHolder)
    {
        TosuDataHolder = tosuDataHolder;
        Updater = new MmfUpdater(tosuDataHolder);
        _configFileUpdateTimer.Interval = 500;
        _configFileUpdateTimer.AutoReset = true;
        _mmfConfigWriteWatcher.FileWrote += (_, _, _) => Load();
        _overlayConfigWriteWatcher.FileWrote += (_, _, _) => Load();
        _configFileUpdateTimer.Elapsed += (_, _) =>
        {
            lock (_locker)
            {
                _mmfConfigWriteWatcher.Check();
                _overlayConfigWriteWatcher.Check();
            }
        };
        
        _configFileUpdateTimer.Start();
        Updater = new MmfUpdater(TosuDataHolder);
    }

    private bool _hasLoaded;
    public bool Loaded => _hasLoaded;
    private void Load()
    {
        var currentDir = Path.GetDirectoryName(typeof(MmfManager).Assembly.Location) ?? "";
        var mmfConfigPath = Path.Combine(currentDir, MmfConfigFile);
        var overlayMmfConfigPath = Path.Combine(currentDir, OverlayMmfConfigFile);
        Updater.Clear();
        ProcessMmfConfig(mmfConfigPath);
        
        var overlayMmfCfg = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(overlayMmfConfigPath));
        ProcessOverlayConfig(overlayMmfCfg ?? []);
        if (!_hasLoaded)
        {
            _hasLoaded = true;
        }
    }
    private void ProcessMmfConfig(string mmfConfigPath)
    {
        JsonMmfConfigReader configReader = new JsonMmfConfigReader();
        configReader.LoadConfig(mmfConfigPath);
        var config = configReader.Config;
        ExpressionMmfItemCollectionBuilder builder = new ExpressionMmfItemCollectionBuilder();
        var builtMmf = builder.Build(config);
        MmfItems = builtMmf;
        foreach (var mmfItem in builtMmf)
        {
            Updater.AddMmf(mmfItem);
        } 
    }
    
    private void ProcessOverlayConfig(JArray overlayMmfCfg)
    {
        foreach (var configItem in Setting.OverlayConfigs.OverlayConfigItems)
        {
            configItem.Visibility = false;
        }
        
        Setting.OverlayConfigs.OverlayConfigItems.Clear();
        foreach (var cfg in overlayMmfCfg)
        {
            var item = cfg.ToObject<OverlayConfigItem>();
            if (item == null)
            {
                continue;
            }
            
            Setting.OverlayConfigs.OverlayConfigItems.Add(item);
        }
    }

    private Timer _updateTimer = new Timer();
    private Dictionary<IMmfItem, string> _originalMmfConfigNames = new Dictionary<IMmfItem, string>();
    private Dictionary<OverlayConfigItem, string> _originalOverlayConfigNames = new Dictionary<OverlayConfigItem, string>();

    private void NormalizeMmfConfigNames(IMmfNameNormalizer nameNormalizer, params object[] args)
    {
        if (!_hasLoaded)
        {
            return;
        }
        
        foreach (var mmfItem in MmfItems)
        {
            if (!_originalMmfConfigNames.TryGetValue(mmfItem, out var name))
            {
                name = mmfItem.MmfName;
                _originalMmfConfigNames.Add(mmfItem, name);
            }

            if (string.IsNullOrEmpty(name))
            {
                name = mmfItem.MmfName;
                _originalMmfConfigNames[mmfItem] = name;
            }
            
            mmfItem.MmfName = nameNormalizer.Normalize(name, args);
        }

    }
    
    private void NormalizeOverlayConfigNames(IMmfNameNormalizer nameNormalizer, params object[] args)
    {
        foreach (var mmfItem in Setting.OverlayConfigs.OverlayConfigItems)
        {
            if (!_originalOverlayConfigNames.TryGetValue(mmfItem, out var name))
            {
                name = mmfItem.Mmf;
                _originalOverlayConfigNames.Add(mmfItem, name);
            }

            if (string.IsNullOrEmpty(name))
            {
                name = mmfItem.Mmf;
                _originalOverlayConfigNames[mmfItem] = name;
            }
            
            mmfItem.Mmf = nameNormalizer.Normalize(name, args);
        }
    }
    public void NormalizeMmfNames(IMmfNameNormalizer nameNormalizer, params object[] args)
    {
        NormalizeOverlayConfigNames(nameNormalizer, args);
        NormalizeMmfConfigNames(nameNormalizer, args);
        _configFileUpdateTimer.Stop();
    }

    public void ReloadConfig() => Load();
    public void UpdateAll()
    {
        if (!TosuDataHolder.IsInitialized() || TosuDataHolder.Value == null)
        {
            return;
        }
        
        Updater.UpdateAll(TosuDataHolder.Value);
    }

    private static readonly object StaticLocker = new object();

    private static Dictionary<IValueHolder<TosuData>, MmfManager> _instances =
        new Dictionary<IValueHolder<TosuData>, MmfManager>();

    public static MmfManager GetInstance(IValueHolder<TosuData> tosuDataHolder)
    {

        lock (StaticLocker)
        {
            if (_instances.TryGetValue(tosuDataHolder, out var stored))
            {
                return stored;
            }
            
            MmfManager mmfManager = new MmfManager(tosuDataHolder);
            _instances.Add(tosuDataHolder, mmfManager);
            return mmfManager;
        }
    }
    
    public static MmfManager CreateInstance(IValueHolder<TosuData> gosuDataHolder)
    {
        MmfManager mmfManager = new MmfManager(gosuDataHolder);
        return mmfManager;
    }

}