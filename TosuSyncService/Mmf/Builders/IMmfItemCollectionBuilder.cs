using HsManCommonLibrary.NestedValues;

namespace TosuSyncService.Mmf.Builders;

public interface IMmfItemCollectionBuilder
{
    IMmfItem[] Build(INestedValueStore? configObj);
}