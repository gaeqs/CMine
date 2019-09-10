using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockAir : Block{
        public BlockAir(Chunk chunk, Vector3i position)
            : base("default:air", null, chunk, position, Color4.Transparent, true) {
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockAir(chunk, position);
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return false;
        }

        public override void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            //Air blocks are empty, no need to load anything.
        }

        public override void Save(Stream stream, BinaryFormatter formatter) {
            //Air blocks are empty, no need to save anything.
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
        }
        
        public override bool CanLightPassThrough(BlockFace face) {
            return true;
        }
        
        public override bool CanLightBePassedFrom(BlockFace face, Block from) {
            return true;
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
        }

        public override void OnSelfLightChange() {
        }
    }
}