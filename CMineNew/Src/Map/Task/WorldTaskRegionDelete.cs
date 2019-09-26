using System;
using CMineNew.Geometry;

namespace CMineNew.Map.Task{
    public class WorldTaskRegionDelete : WorldTask{
        private ChunkRegion _region;

        public WorldTaskRegionDelete(ChunkRegion region) : base(CMine.TicksPerSecond) {
            _region = region;
        }

        public override void Run() {
            if (_region.DeleteIfEmpty()) {
                if (_region.World.ChunkRegions.TryRemove(_region.Position, out _)) {
                    Console.WriteLine("Region " + _region.Position + " deleted");
                }

                if (_region.World.Regions2d.TryRemove(new Vector2i(_region.Position.X, _region.Position.Z),
                    out var region2d)) {
                    Console.WriteLine("Region2d " + region2d.Position + " deleted");
                }
            }
        }
    }
}