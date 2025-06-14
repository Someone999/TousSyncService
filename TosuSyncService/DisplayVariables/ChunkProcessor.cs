using System.Buffers;

namespace TosuSyncService.DisplayVariables;

public class ChunkProcessor
{
    private ArrayPool<double> _pool = ArrayPool<double>.Create();
}