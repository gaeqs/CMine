namespace CMineNew.Map.BlockData.Render{
    public abstract class BlockRender{
        public abstract void AddData(int mapper, Block block, int blockLight, int sunlight);

        public abstract void RemoveData(int mapper, Block block);

        public abstract void Draw(bool first);

        public abstract void DrawAfterPostRender(bool first);

        public abstract void CleanUp();
    }
}