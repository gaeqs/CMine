using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Task.Type{
    public class WorldTaskExpandWater : WorldTask{
        private readonly World _world;
        private readonly Vector3i _position;

        public WorldTaskExpandWater(World world, Vector3i position) : base(BlockWater.ExpandTicks) {
            _world = world;
            _position = position;
        }

        public override void Run() {
            var block = _world.GetBlock(_position);
            if (!(block is BlockWater water)) return;
            if(water.Removing) return;
            var neighbours = block.Chunk.GetNeighbourBlocks(new Block[6], _position,
                _position - (block.Chunk.Position << 4));

            var neighbourDown = neighbours[(int) BlockFace.Down];
            var cantBeExpanded = (neighbourDown is BlockWater || neighbourDown is BlockTallGrass ||
                                 neighbourDown is BlockAir) && water.Parent != water.Position;

            foreach (var blockFace in BlockFaceMethods.All) {
                if (blockFace == BlockFace.Up) continue;
                if ((water.WaterLevel == 0 || cantBeExpanded) && blockFace != BlockFace.Down) continue;
                var target = neighbours[(int) blockFace];
                //Must delete this line later on.
                if (target == null) return;
                if (target is BlockWater waterTarget) {
                    if ((waterTarget.WaterLevel >= water.WaterLevel || 
                         waterTarget.WaterLevel > water.WaterLevel && waterTarget.WaterLevel == 0) &&
                        (blockFace != BlockFace.Down || waterTarget.Parent == waterTarget.Position)) continue;

                    //Set parent
                    var parent = _world.GetBlock(waterTarget.Parent);
                    if (parent != null && parent is BlockWater waterParent) {
                        waterParent.Children.Remove(waterTarget.Position);
                    }
                }

                if (!(target is BlockAir) && !(target is BlockTallGrass) && !(target is BlockWater)) continue;
                var pos = block.Position + BlockFaceMethods.GetRelative(blockFace);
                var set = _world.SetBlock(new BlockSnapshotWater(blockFace == BlockFace.Down
                    ? BlockWater.MaxWaterLevel
                    : water.WaterLevel - 1), pos);
                ((BlockWater) set).Parent = block.Position;
                water.Children.Add(set.Position);
            }
        }
    }
}