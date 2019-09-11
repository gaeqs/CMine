using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Snapshot.Interface;
using CMineNew.Map.BlockData.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotTorch : BlockSnapshot{

        public BlockSnapshotTorch() : base("default:torch") {
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(TorchBlockModel.Key);
        public override bool Passable => true;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockTorch(chunk, position) ;
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            var block = world.GetBlock(position + new Vector3i(0, -1, 0));
            return block.IsFaceOpaque(BlockFace.Up);
        }

        public override BlockSnapshot Clone() {
            return new BlockSnapshotTorch();
        }
    }
}