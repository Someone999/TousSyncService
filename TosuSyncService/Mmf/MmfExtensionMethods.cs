using osuToolsV2.Rulesets.Legacy;
using TosuSyncService.Model.Settings;

namespace TosuSyncService.Mmf;

public static class MmfExtensionMethods
{
    private static void InitMap()
    {
        var stateNames = Enum.GetNames<MmfState>();
        foreach (var stateName in stateNames)
        {
            var mmfStateVal = Enum.Parse<MmfState>(stateName);
            var gosuVal = Enum.Parse<TosuGameState>(stateName);
            _statesMap.Add(mmfStateVal, gosuVal);
        }
    }
    
    static MmfExtensionMethods()
    {
        if (_statesMap != null)
        {
            return;
        }
        
        _statesMap = new Dictionary<MmfState, TosuGameState>();
        InitMap();
    }
    
    private static Dictionary<MmfState, TosuGameState> _statesMap;

    private static Dictionary<MmfRuleset, LegacyRuleset> _rulesetMap = new Dictionary<MmfRuleset, LegacyRuleset>()
    {
        { MmfRuleset.Osu, LegacyRuleset.Osu},
        { MmfRuleset.Taiko, LegacyRuleset.Taiko},
        { MmfRuleset.CatchTheBeat, LegacyRuleset.Catch},
        { MmfRuleset.Mania, LegacyRuleset.Mania}
        
    };

    public static TosuGameState[] GetAsGosuStates(this MmfState mmfState)
    {
        var names = Enum.GetNames<MmfState>();
        List<TosuGameState> states = new List<TosuGameState>();
        int mmfStateInt = (int)mmfState;
        for (int i = 0; i < names.Length; i++)
        {
            int mask = 1 << i;
            int rawEnumVal = mmfStateInt & mask;
            if (rawEnumVal == mask)
            {
                states.Add(_statesMap[(MmfState)rawEnumVal]);
            }
        }

        return states.ToArray();
    }
    
    public static LegacyRuleset[] GetAsGosuRulesets(this MmfRuleset mmfRuleset)
    {
        var names = Enum.GetNames<MmfState>();
        List<LegacyRuleset> legacyRulesets = new List<LegacyRuleset>();
        int mmfRulesetInt = (int)mmfRuleset;
        for (int i = 0; i < names.Length; i++)
        {
            int mask = 1 << i;
            int rawEnumVal = mmfRulesetInt & mask;
            if (rawEnumVal == mask)
            {
                legacyRulesets.Add(_rulesetMap[(MmfRuleset)rawEnumVal]);
            }
        }

        return legacyRulesets.ToArray();
    }
    
}