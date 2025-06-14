namespace TosuSyncService.Native;

public interface ILibraryInjector
{
    void Inject(string libraryPath, int pid);
}