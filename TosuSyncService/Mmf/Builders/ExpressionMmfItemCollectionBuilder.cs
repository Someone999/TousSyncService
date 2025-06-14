using System.Runtime.Versioning;
using HsManCommonLibrary.NestedValues;

namespace TosuSyncService.Mmf.Builders;

[SupportedOSPlatform("windows")]
public class ExpressionMmfItemCollectionBuilder : IMmfItemCollectionBuilder
{
    public IMmfItem[] Build(INestedValueStore? configObj)
    {
        var topValue = configObj?.GetValue();
        if (topValue is not Dictionary<string, INestedValueStore> valueStores)
        {
            return [];
        }
        
        List<IMmfItem> mmfItems = new List<IMmfItem>();
        Dictionary<string, MmfOutputPatternItem> outputPatternFiles = new Dictionary<string, MmfOutputPatternItem>();
        var outputPatternFilesStore = valueStores["outputPatternFiles"].GetValueAs<List<INestedValueStore>>();
        if (outputPatternFilesStore != null)
        {
            foreach (var outputPatternFile in outputPatternFilesStore)
            {
                var val = outputPatternFile.GetValue();
                if (val is not Dictionary<string, INestedValueStore> dictionary)
                {
                    continue;
                }

                var key = dictionary["name"].GetValueAs<string>() ?? "";
                var path = dictionary["path"].GetValueAs<string>() ?? "";
                MmfOutputPatternItem fileItem = new MmfOutputPatternItem()
                {
                    Name = key,
                    Path = path
                };

                outputPatternFiles.Add(key, fileItem);
            }
        }

        var mmfDefinitions = valueStores["mmfDefinitions"].GetValue();
        if (mmfDefinitions is not List<INestedValueStore> mmfDefinitionList)
        {
            return [];
        }
        
        foreach (var nestedValueStore1 in mmfDefinitionList)
        {
            var nestedValueStore = (INestedValueStoreAccessor) nestedValueStore1;
            var mmfNameStore =
                nestedValueStore.GetMemberValueAsValueHolder<string>("mmfName");
            var patternFileNameStore =
                nestedValueStore.GetMemberValueAsValueHolder<string>("patternFileName");
            var availableStateStore =
                nestedValueStore.GetMemberValueAsValueHolder<string>("availableState");
            var availableRulesetStore =
                nestedValueStore.GetMemberValueAsValueHolder<string>("availableRuleset");
            var enabledStore =
                nestedValueStore.GetMemberValueAsValueHolder<bool>("enabled");
            var symbolTableNameStore =
                nestedValueStore.GetMemberValueAsValueHolder<string>("symbolTableName");

            var missingMember = !mmfNameStore.IsInitialized() || !patternFileNameStore.IsInitialized() ||
                                !availableStateStore.IsInitialized() || !availableRulesetStore.IsInitialized() ||
                                !enabledStore.IsInitialized();

            if (missingMember)
            {
                continue;
            }

            var patternFileName = patternFileNameStore.Value;
            if (string.IsNullOrEmpty(patternFileName) || string.IsNullOrEmpty(mmfNameStore.Value)
                                                      || string.IsNullOrEmpty(symbolTableNameStore.Value))
            {
                continue;
            }

            bool isRefStart = patternFileName.StartsWith("ref(");
            bool isRefEnd = patternFileName.EndsWith(')');
            string outputPatternFilePath = patternFileName;
            if (isRefStart && isRefEnd)
            {
                var key = patternFileName[4..^1];
                if (!outputPatternFiles.TryGetValue(key, out var value))
                {
                    continue;
                }

                outputPatternFilePath = value.Path;
            }

            var availableState = Enum.Parse<MmfState>(availableStateStore.Value ?? "");
            var availableRuleset = Enum.Parse<MmfRuleset>(availableRulesetStore.Value ?? "");
            var enabled = enabledStore.Value;
            var symbolTableName = symbolTableNameStore.Value;


            ExpressionMmfItem expressionMmfItem =
                new ExpressionMmfItem(mmfNameStore.Value, outputPatternFilePath, symbolTableName)
                {
                    AvailableState = availableState,
                    AvailableRuleset = availableRuleset,
                    Enabled = enabled
                };

            mmfItems.Add(expressionMmfItem);
        }

        return mmfItems.ToArray();
    }
}