namespace CMineNew.Map.Task{
    public class WorldTaskRegionDelete : WorldTask{
        private ChunkRegion _region;

        public WorldTaskRegionDelete(ChunkRegion region) : base(0) {
            _region = region;
        }

        public override void Run() {
            _region.DeleteIfEmpty();
        }
    }
}