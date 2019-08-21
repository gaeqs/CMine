using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotTallGrass : BlockSnapshot{
        public BlockSnapshotTallGrass() : base("default:tall_grass") {
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(CrossBlockModel.Key);
        public override bool Passable => true;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            var block = world.GetBlock(position + new Vector3i(0, -1, 0));
            return block is BlockGrass;
        }
    }
}