using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public abstract class BlockStaticData{
        public BlockStaticData(string id, BlockModel blockModel, bool passable,
            float blockHeight, float blockYOffset, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction) {
            Id = id;
            BlockModel = blockModel;
            Passable = passable;
            BlockHeight = blockHeight;
            BlockYOffset = blockYOffset;
            LightSource = lightSource;
            LightSourceLight = lightSourceLight;
            BlockLightPassReduction = blockLightPassReduction;
            SunlightPassReduction = sunlightPassReduction;
        }

        public string Id { get; }

        public BlockModel BlockModel { get; protected set; }

        public bool Passable { get; }

        public float BlockHeight { get; }

        public float BlockYOffset { get; }

        public bool LightSource { get; }

        public sbyte LightSourceLight { get; }

        public sbyte BlockLightPassReduction { get; }

        public sbyte SunlightPassReduction { get; }

        public abstract int GetTextureIndex(BlockFace face);
    }
}