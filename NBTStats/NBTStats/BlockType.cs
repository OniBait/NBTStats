using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NBTStats
{
    public class BlockType
    {
        public int Id { get; private set; }
        public int Data { get; private set; }

        private readonly ConcurrentDictionary<int, int> yvalues = new ConcurrentDictionary<int, int>();

        public BlockType(string str)
        {
            var values = str.Split(':');
            Id = int.Parse(values[0]);
            if (values.Length > 1)
            {
                Data = int.Parse(values[1]);
            }
        }

        public void AddYValue(int y)
        {
            yvalues.AddOrUpdate(y, i => 1, (i, j) => j + 1);
        }

        public void PrintAvg(int totalChunks)
        {
            float count = yvalues.Values.Sum();
            Console.WriteLine("Block: {0}:{1} Avg: {2:0.00} ", Id, Data, count / totalChunks);
            
        }

        public void PrintValues(int totalChunks)
        {
            float count = yvalues.Values.Sum();
            var levels = yvalues.Keys.ToList();

            Console.WriteLine("Block: {0}:{1}", Id, Data);
            Console.WriteLine("Count: {0}", count);
            Console.WriteLine("Avg/Chunk: {0:0.00}", count / totalChunks);
            Console.WriteLine("Avg Y: {0:0.00}", yvalues.Select(kv => kv.Key * kv.Value).Sum() / count);
            Console.WriteLine("Min Y: {0}", levels.Min());
            Console.WriteLine("Max Y: {0}", levels.Max());
            Console.WriteLine("Median Y: {0}", levels[levels.Count / 2]);

            Console.WriteLine();
            var min = (int)(Math.Floor(levels.Min() / 10d) * 10);
            var max = (int)(Math.Ceiling(levels.Max() / 10d) * 10);

            for (int i = min; i < max; i += 10)
            {
                var rangeCount = yvalues.Where(kv => kv.Key >= i && kv.Key < i + 10).Sum(kv => kv.Value);
                Console.WriteLine("Levels {0}-{1} Pct:{2:P}", i, i + 10, rangeCount / count);
            }

            Console.WriteLine();
            foreach (var yvalue in yvalues)
            {
                Console.WriteLine("Level: {0} Pct: {1:P}", yvalue.Key, yvalue.Value / count);
            }
        }
    }
}