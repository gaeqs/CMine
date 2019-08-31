using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Snapshot.Interface;
using CMineNew.Map.BlockData.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotGrass : BlockSnapshot, IGrass{
        private Color4 _grassColor;

        public BlockSnapshotGrass(Color4 grassColor) : base("default:grass") {
            _grassColor = grassColor;
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(CubicBlockModel.Key);
        public override bool Passable => false;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position) {TextureFilter = _grassColor};
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }

        public override BlockSnapshot Clone() {
            return new BlockSnapshotGrass(_grassColor);
        }

        public Color4 GrassColor {
            get => _grassColor;
            set => _grassColor = value;
        }
    }
}