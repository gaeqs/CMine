using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;

namespace CMineNew.Map {
    public class SunlightData {
        private readonly World2dRegion _region;
        private readonly Vector2i _worldPosition;

        /// <summary>
        /// Represents where the light level ends.
        /// The value stored here is exclude: if the light level 14
        /// has the value 50 stored. The light ends at the block 51, inclusive.
        /// </summary>
        private readonly int[] _lightHeight;

        public SunlightData(World2dRegion region, Vector2i position) {
            _region = region;
            _worldPosition = position + (region.Position << World2dRegion.WorldPositionShift);
            _lightHeight = new int[15];
            for (var i = 0; i < _lightHeight.Length; i++) {
                _lightHeight[i] = int.MinValue;
            }
        }

        public sbyte GetLightFor(int y) {
            for (var i = _lightHeight.Length - 1; i >= 0; i--) {
                var lightY = _lightHeight[i];
                if (lightY < y) return (sbyte) (i + 1);
            }

            return 0;
        }

        public void SetBlock(int y, int lightReduction, Block block) {
            var previousLight = GetLightFor(y);
            var upperLight = GetLightFor(y + 1);
            var nextLight = (sbyte) Math.Max(upperLight - lightReduction, 0);

            block.BlockLight.LinearSunlight = nextLight;
            block.BlockLight.Sunlight = block.BlockLight.LinearSunlight;
            block.BlockLight.SunlightSource = block.Position;

            if (previousLight == nextLight) {
                return;
            }

            //nextLight >= previousLight || _lightHeight[0] == int.MinValue || Math.Abs(y - _lightHeight[0]) > 100)
            Opaque(block, y, upperLight, nextLight);
        }

        private void Opaque(Block thisBlock, int y, sbyte previousLight, sbyte modifiedLight) {
            var blocks = _region.World.GetVerticalColumn(_worldPosition, y - 1);
            var enumerable = blocks as Block[] ?? blocks.ToArray();

            for (var i = previousLight; i > modifiedLight; i--) {
                _lightHeight[i - 1] = y;
            }

            previousLight = modifiedLight;

            foreach (var block in enumerable) {
                if (block == null) continue;
                var reduction = block.StaticData.SunlightPassReduction;
                if (reduction == 0 || modifiedLight == 0) {
                    block.BlockLight.LinearSunlight = modifiedLight;
                    continue;
                }

                modifiedLight -= reduction;
                if (modifiedLight < 0) modifiedLight = 0;

                for (var i = previousLight; i > modifiedLight; i--) {
                    _lightHeight[i - 1] = block.Position.Y;
                }

                previousLight = modifiedLight;
                block.BlockLight.LinearSunlight = modifiedLight;
            }


            thisBlock.RemoveSunlight();
            foreach (var block in enumerable) {
                block?.RemoveSunlight();
            }

            thisBlock.ExpandSunlight();
            foreach (var block in enumerable) {
                block?.ExpandSunlight();
            }
        }

        public void Save(Stream stream, BinaryFormatter formatter) {
            foreach (var t in _lightHeight) {
                formatter.Serialize(stream, t);
            }
        }

        public void Load(Stream stream, BinaryFormatter formatter) {
            for (var i = 0; i < _lightHeight.Length; i++) {
                _lightHeight[i] = (int) formatter.Deserialize(stream);
            }
        }

        public override string ToString() {
            var s = "";
            for (var i = _lightHeight.Length - 1; i >= 0; i--) {
                s += i + 1 + " -> " + _lightHeight[i] + "\n";
            }

            return s;
        }
    }
}