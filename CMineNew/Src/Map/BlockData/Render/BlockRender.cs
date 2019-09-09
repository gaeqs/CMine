namespace CMineNew.Map.BlockData.Render{
    public abstract class BlockRender{
        public abstract void AddData(int mapper, Block block, int light);

        public abstract void RemoveData(int mapper, Block block);

        public abstract void Draw();

        public abstract void DrawAfterPostRender();

        public abstract void FlushInBackground();

        public abstract void CleanUp();
    }
}