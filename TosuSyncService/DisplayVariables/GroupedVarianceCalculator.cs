using System.Collections.Concurrent;

namespace TosuSyncService.DisplayVariables;

public class GroupedVarianceCalculator
{
    private class NumberEqualityDoubleArrayComparer : IEqualityComparer<double[]>
    {
        public bool Equals(double[]? x, double[]? y)
        {
            
            if (x == null && y == null)
            {
                
                return true;
            }
            
            if (x == null || y == null)
            {
                return false;
            }

            return x.Length == y.Length && x.SequenceEqual(y);
        }

        public int GetHashCode(double[] obj)
        {
            HashCode hashCode = new HashCode();
            foreach (var d in obj)
            {
                hashCode.Add(d);
            }

            return hashCode.ToHashCode();
        }

        public static IEqualityComparer<double[]> Instance { get; } = new NumberEqualityDoubleArrayComparer();
    }

    private class StatisticResult
    {
        public double Sum = 0;
        public double Average = 0;
        public double Variance = 0;
        public int DataCount = 0;
        public double Min = 0;
        public double Max = 0;

        public void Add(StatisticResult result)
        {
            
            // Update sum and data count
            double combinedSum = Sum + result.Sum;
            int combinedCount = DataCount + result.DataCount;

            // Calculate new average
            double combinedAverage = combinedSum / combinedCount;

            // Calculate combined variance using the formula
            double combinedVariance = 
                (DataCount * (Variance + Math.Pow(Average, 2)) +
                 result.DataCount * (result.Variance + Math.Pow(result.Average, 2))) / combinedCount
                - Math.Pow(combinedAverage, 2);

            // Update the current TempResult with the combined results
            Sum = combinedSum;
            DataCount = combinedCount;
            Average = combinedAverage;
            Variance = combinedVariance;
            Min = Math.Min(Min, result.Min);
            Max = Math.Max(Max, result.Max);
        }

        public void Clear()
        {
            Sum = 0;
            DataCount = 0;
            Average = 0;
            Variance = 0;
            Min = 0;
            Max = 0;
        }
    }

    private ConcurrentDictionary<double[], StatisticResult> _resultsCache = new(NumberEqualityDoubleArrayComparer.Instance);
    private StatisticResult _overallResult = new StatisticResult();
    private double[] _cache = [];
    private static readonly StatisticResult EmptyResult = new StatisticResult();

    private StatisticResult ComputeChunk(double[] chunk, int realCount, bool useCache = true)
    {
        if (useCache && _resultsCache.TryGetValue(chunk, out var result))
        {
            return result;
        }

        double sum = chunk.Sum();
        if (sum == 0)
        {
            return new StatisticResult()
            {
                DataCount = chunk.Length
            };
        }
        
        double average = sum / chunk.Length;
        double variance = chunk.Select(d => Math.Pow(d - average, 2)).Sum() / chunk.Length;
        double min = chunk.Min();
        double max = chunk.Max();
        result = new StatisticResult
        {
            Sum = sum,
            Average = average,
            Variance = variance,
            DataCount = realCount,
            Min = min,
            Max = max
        };

        if (!useCache)
        {
            return result;
        }
        
        _resultsCache.TryAdd(chunk, result);
        return result;
    }
    
    
    private void ComputeOverallChunkedParallel(double[] array)
    {
        if (NumberEqualityDoubleArrayComparer.Instance.Equals(array, _cache))
        {
            return;
        }


        StatisticResult currentResult = new StatisticResult();
        var chunkSize = Math.Min(Environment.ProcessorCount * 8, 64);
        var chunked = array.Chunk(chunkSize);
        //foreach (var arr in chunked)
        //{
        //    var useCache = arr.Length == chunkSize;
        //    var result = ComputeChunk(arr, arr.Length, useCache);
        //    currentResult.Add(result);
        //}
        
        Parallel.ForEach(chunked, arr =>
        {
            var useCache = arr.Length == chunkSize;
            var result = ComputeChunk(arr, arr.Length, useCache);
            currentResult.Add(result);
        });

        _cache = array.ToArray();
        _overallResult = currentResult;
    }

    private void ComputeOverallNoChunk(double[] array)
    {
        _overallResult.Average = array.Average();
        _overallResult.DataCount = array.Length;
        _overallResult.Max = array.Max();
        _overallResult.Min = array.Min();
        _overallResult.Sum = array.Sum();
        _overallResult.Variance = array.Sum(x => Math.Pow(x - Average, 2)) / _overallResult.DataCount;
    }

    public void Reset()
    {
        _resultsCache.Clear();
        _overallResult.Clear();
    }

    public void Update(double[] array)
    {
        ComputeOverallChunkedParallel(array);
    }

    public double Sum => _overallResult.Sum;
    public double Average => _overallResult.Average;
    public double Variance => _overallResult.Variance;
    public double Max => _overallResult.Max;
    public double Min => _overallResult.Min;
}