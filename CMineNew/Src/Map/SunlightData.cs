using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;

namespace CMineNew.Map{
    public class SunlightData{
        private readonly World2dRegion _region;
        private readonly Vector2i _position;

        /// <summary>
        /// Represents where the light level ends.
        /// The value stored here is exclude: if the light level 14
        /// has the value 50 stored. The light ends at the block 51, inclusive.
        /// </summary>
        private readonly int[] _lightMinHeight;

        public SunlightData(World2dRegion region, Vector2i position) {
            _region = region;
            _position = position;
            _lightMinHeight = new int[15];
            for (var i = 0; i < _lightMinHeight.Length; i++) {
                _lightMinHeight[i] = int.MinValue;
            }
        }

        public int GetLightFor(int y) {
            for (var i = _lightMinHeight.Length - 1; i >= 0; i--) {
                var lightY = _lightMinHeight[i];
                if (lightY < y) return i;
            }

            return 0;
        }

        public void SetBlock(int y, int lightReduction) {
            var list = RemovePreviousBlock(y,  lightReduction);

            var old0Height = _lightMinHeight[0];
            
            if (lightReduction > 0) {
                var light = GetLightFor(y);
                var newLight = Math.Max(0, light - lightReduction) - 1;
                if (newLight == -1 && _lightMinHeight[0] > y) return;

                for (var i = 0; i <= newLight; i++) {
                    _lightMinHeight[Math.Max(0, i - lightReduction)] = _lightMinHeight[i];
                }

                for (var i = newLight + 1; i <= light; i++) {
                    _lightMinHeight[i] = y;
                }
            }
            
            if(lightReduction == 0 && list.Count == 0) return;
            
            //Update block
            

           
            var chunkPositionInRegion = _position >> Chunk.WorldPositionShift;
            var worldPositionInChunk = _position - (chunkPositionInRegion << Chunk.WorldPositionShift);
            var regionPosition = _region.Position;
            var world = _region.World;
            var regionY = y >> ChunkRegion.WorldPositionShift;
            var chunkY = y >> Chunk.WorldPositionShift;
            var regions = new List<ChunkRegion>();
            lock (world.RegionsLock) {
                regions.AddRange(from region in world.ChunkRegions.Values
                    let rPos = region.Position
                    where rPos.X == regionPosition.X && rPos.Z == regionPosition.Y && rPos.Y >= regionY
                    select region);
            }
            
            regions.Sort((r1, r2) => r2.Position.Y - r1.Position.Y);

            foreach (var chunks in regions.Select(region => region.Chunks)) {
                for (var cy = 3; cy >= 0; cy--) {
                    var chunk = chunks[chunkPositionInRegion.X, cy, chunkPositionInRegion.Y];
                    if (chunk == null || chunk.Position.Y > chunkY) continue;
                    var blocks = chunk.Blocks;
                    for (var by = 15; by >= 0; by--) {
                        var block = blocks[worldPositionInChunk.X, by, worldPositionInChunk.Y];
                        if (block.Position.Y >= y || block.Position.Y <= old0Height) continue;
                        //TODO UPDATE LINEAR SUNLIGHT
                        block.BlockLight.LinearSunlight = GetLightFor(block.Position.Y);
                        block.TriggerLightChange();
                    }
                }
            }

            foreach (var block in list) {
                block.TriggerLightChange();
            }
        }

        private List<Block> RemovePreviousBlock(int y, int newReduction) {
            var add = 0;
            for (var i = _lightMinHeight.Length - 1; i >= 0; i--) {
                var lightY = _lightMinHeight[i];
                if (lightY == y) {
                    add++;
                }
                
                _lightMinHeight[Math.Min(14, i + add)] = lightY;
            }

            if (add == 15) {
                add--;
            }
            
            return add < 1
                ? new List<Block>()
                : Check(add, _lightMinHeight[add], newReduction);
        }

        private List<Block> Check(int lastLight, int y, int newReduction) {

            var list = new List<Block>();
            var regionY = y >> ChunkRegion.WorldPositionShift;
            var chunkY = y >> Chunk.WorldPositionShift;
            var regionPosition = _region.Position;
            var chunkPositionInRegion = _position >> Chunk.WorldPositionShift;
            var worldPositionInChunk = _position - (chunkPositionInRegion << Chunk.WorldPositionShift);
            var world = _region.World;

            var regions = new List<ChunkRegion>();
            lock (world.RegionsLock) {
                regions.AddRange(from region in world.ChunkRegions.Values
                    let rPos = region.Position
                    where rPos.X == regionPosition.X && rPos.Z == regionPosition.Y && rPos.Y >= regionY
                    select region);
            }

            regions.Sort((r1, r2) => r2.Position.Y - r1.Position.Y);

            foreach (var chunks in regions.Select(region => region.Chunks)) {
                for (var cy = 3; cy >= 0; cy--) {
                    if (lastLight <= newReduction) return list;
                    var chunk = chunks[chunkPositionInRegion.X, cy, chunkPositionInRegion.Y];
                    if (chunk == null || chunk.Position.Y > chunkY) continue;
                    var blocks = chunk.Blocks;
                    for (var by = 15; by >= 0; by--) {
                        if (lastLight <= newReduction) return list;
                        var block = blocks[worldPositionInChunk.X, by, worldPositionInChunk.Y];
                        if (block.Position.Y >= y) continue;
                        var bLight = block.BlockLight;
                        var reduction = bLight.SunlightPassReduction;

                        if (reduction > 0) {
                            var nLight = Math.Max(-1, lastLight - reduction);
                            for (var i = lastLight; i > nLight; i--) {
                                _lightMinHeight[i] = block.Position.Y;
                            }

                            lastLight = nLight;
                        }

                        //TODO UPDATE LINEAR SUNLIGHT
                        bLight.LinearSunlight = lastLight + 1;
                        list.Add(block);
                    }
                }
            }

            return list;
        }

        public void Save(Stream stream, BinaryFormatter formatter) {
            foreach (var t in _lightMinHeight) {
                formatter.Serialize(stream, t);
            }
        }

        public void Load(Stream stream, BinaryFormatter formatter) {
            for (var i = 0; i < _lightMinHeight.Length; i++) {
                _lightMinHeight[i] = (int) formatter.Deserialize(stream);
            }
        }

        public override string ToString() {
            var s = "";
            for (var i = _lightMinHeight.Length - 1; i >= 0; i--) {
                s += i + 1 + " -> " + _lightMinHeight[i] + "\n";
            }

            return s;
        }
    }
}