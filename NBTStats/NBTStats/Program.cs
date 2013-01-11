using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Substrate;

namespace NBTStats
{
    class Program
    {

        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: BlockReplace <world> <blockids>");
                return;
            }

            var sw = Stopwatch.StartNew();

            var dest = args[0];

            var blockIds = args.Skip(1).Select(arg => new BlockType(arg)).ToList();

            new ChunkProcessor(NbtWorld.Open(dest), blockIds).Run();

            Console.WriteLine("Elapsed time: {0}", sw.Elapsed);
        }

    }
}
