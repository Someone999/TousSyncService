using TosuSyncService.Model;

namespace TosuSyncService.IngameOverlay;

class BreakTimeParser
{
    class BreakTime
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
    
    private readonly TosuData _data;
    private List<BreakTime> _breakTimes = new List<BreakTime>();

    public BreakTimeParser(TosuData data)
    {
        _data = data;
        Parser();
    }

    private void Parser()
    {
        string blockName = "";
        var folders = _data.Folders;
        var beatmapPath = Path.Combine(folders.Songs, _data.DirectPath.BeatmapFile);

        if (!File.Exists(beatmapPath))
        {
            return;
        }
        foreach (string line in File.ReadLines(beatmapPath))
        {
            if (line.StartsWith("["))
            {
                blockName = line.Trim();
            }
            else if (blockName.StartsWith("[Events]"))
            {
                IList<string> parms = line.Split(',');
                if (parms[0].StartsWith("2"))
                {
                    _breakTimes.Add(new BreakTime()
                    {
                        Start = int.Parse(parms[1]),
                        End = int.Parse(parms[2])
                    });
                }
                else if(line==string.Empty)
                {
                    break;
                }
            }
        }
    }

    public bool InBreakTime(int time)
    {
        return _breakTimes.Exists(breakTime => time > breakTime.Start && time < breakTime.End);
    }
}