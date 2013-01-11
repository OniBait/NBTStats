using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Substrate;
using Substrate.Core;

namespace NBTStats
{
    public class ChunkProcessor
    {
        private readonly NbtWorld _world;
        private readonly Dictionary<int, Dictionary<int, BlockType>> _blockIds;
        private readonly List<BlockType> _blockArgs;

        private int _processedChunks;
        private int _totalChunks;

        private static object SyncRoot = new object();

        public ChunkProcessor(NbtWorld world, List<BlockType> blockArgs)
        {
            _world = world;
            _blockIds = new Dictionary<int, Dictionary<int, BlockType>>();
            foreach (var blockArg in blockArgs)
            {
                if (_blockIds.ContainsKey(blockArg.Id))
                {
                    _blockIds[blockArg.Id][blockArg.Data] = blockArg;
                    continue;
                }
                _blockIds[blockArg.Id] = new Dictionary<int, BlockType>();
                _blockIds[blockArg.Id][blockArg.Data] = blockArg;
            }


            _blockArgs = blockArgs;
        }

        public void Run()
        {
            Console.Clear();
            var cm = _world.GetChunkManager();

            _totalChunks = cm.Count();

            Console.WriteLine("# Chunks: {0}", _totalChunks);

            Task.WaitAll(cm.Select(chunkRef => Task.Factory.StartNew(() => ProcessChunk(chunkRef))).ToArray());

            Console.WriteLine();

            foreach (var block in _blockArgs)
            {
                block.PrintValues(_totalChunks);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void ProcessChunk(IChunk chunk1)
        {
            var xdim = chunk1.Blocks.XDim;
            var ydim = chunk1.Blocks.YDim;
            var zdim = chunk1.Blocks.ZDim;

            for (var x = 0; x < xdim; x++)
            {
                for (var z = 0; z < zdim; z++)
                {
                    for (var y = 0; y < ydim; y++)
                    {
                        var id = chunk1.Blocks.GetID(x, y, z);
                        var blocks = _blockIds.Get(id);
                        if (blocks == null) continue;

                        var data = chunk1.Blocks.GetData(x, y, z);
                        var block = blocks.Get(data);
                        if (block == null) continue;
                        
                        block.AddYValue(y);
                    }
                }
            }

            Interlocked.Increment(ref _processedChunks);
            if (_processedChunks%10 != 0 && _processedChunks < _totalChunks - 20) return;

            lock (SyncRoot)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 1;
                foreach (var blockArg in _blockArgs)
                {
                    blockArg.PrintAvg(_processedChunks);
                }
                Console.WriteLine("Block # {0} {1:P} complete", _processedChunks, ((float)_processedChunks)/_totalChunks);
            }
        }
        
    }
}