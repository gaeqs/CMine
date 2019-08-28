using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Unloaded{
    public class UnloadedChunkGenerationBlock{

        private BlockSnapshot _snapshot;
        private bool _overrideBlocks;

        public UnloadedChunkGenerationBlock(BlockSnapshot snapshot, bool overrideBlocks) {
            _snapshot = snapshot;
            _overrideBlocks = overrideBlocks;
        }

        public BlockSnapshot Snapshot => _snapshot;

        public bool OverrideBlocks => _overrideBlocks;
    }
}