using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Unloaded{
    public class UnloadedChunkGenerationBlock{
        private BlockSnapshot _snapshot;
        private bool _overrideBlocks;

        public UnloadedChunkGenerationBlock(BlockSnapshot snapshot, bool overrideBlocks) {
            _snapshot = snapshot;
            _overrideBlocks = overrideBlocks;
        }

        public UnloadedChunkGenerationBlock(Stream stream, BinaryFormatter formatter) {
            _overrideBlocks = (bool) formatter.Deserialize(stream);
            if (BlockManager.TryGet((string) formatter.Deserialize(stream), out _snapshot)) {
                _snapshot.Load(stream, formatter);
            }
            else {
                throw new System.Exception("Block not found while loading an UnloadedChunkGenerationBlock.");
            }
        }

        public BlockSnapshot Snapshot => _snapshot;

        public bool OverrideBlocks => _overrideBlocks;

        public void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _overrideBlocks);
            formatter.Serialize(stream, _snapshot.Id);
            _snapshot.Save(stream, formatter);
        }
    }
}