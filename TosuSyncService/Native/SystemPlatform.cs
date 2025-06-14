namespace TosuSyncService.Native;


public static class SystemPlatform
{
    private static PlatformType GetPlatformType()
    {
        if (OperatingSystem.IsWindows())
        {
            return PlatformType.Windows;
        }

        if (OperatingSystem.IsIOS()
            || OperatingSystem.IsMacOS()
            || OperatingSystem.IsMacCatalyst()
            || OperatingSystem.IsTvOS()
            || OperatingSystem.IsWatchOS())
        {
            return PlatformType.AppleFamily;
        }

        if (OperatingSystem.IsFreeBSD())
        {
            return PlatformType.UnixFamily;
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsAndroid())
        {
            return PlatformType.LinuxFamily;
        }

        return PlatformType.Unknown;
    }

    private static PlatformType? _current;

    public static PlatformType Current
    {
        get
        {
            if (_current != null)
            {
                return _current.Value;
            }

            _current = GetPlatformType();
            return _current.Value;
        }
    }
}
