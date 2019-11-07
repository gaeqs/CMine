using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;

namespace CMineNew.Map {
    public class SunlightData {
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

        public sbyte GetLightFor(int y) {
            for (var i = _lightMinHeight.Length - 1; i >= 0; i--) {
                var lightY = _lightMinHeight[i];
                if (lightY < y) return (sbyte) (i + 1);
            }

            return 0;
        }

        public void SetBlock(int y, int lightReduction) {
            var previousLight = (sbyte) (GetLightFor(y) - 1);
            var upperLight = GetLightFor(y + 1) - 1;
            var nextLight = (sbyte) Math.Max(upperLight - lightReduction, 0);
            if (previousLight == nextLight) return;
            if (nextLight < previousLight) {
                if (_lightMinHeight[0] == int.MinValue || Math.Abs(y - _lightMinHeight[0]) > 100) {
                    Opaque(y, previousLight, nextLight, true);
                }
                else {
                    Opaque(y, previousLight, nextLight, false);
                }
            }
            else {
                Opaque(y, (sbyte) (upperLight + 1), nextLight, true);
            }
        }

        private void Opaque(int y, sbyte previousLight, sbyte modifiedLight, bool modifyAll) {
            for (var i = previousLight - 1; i > modifiedLight; i--) {
                _lightMinHeight[i] = y;
            }

            previousLight = modifiedLight;

            IEnumerable<Block> blocks;

            if (modifyAll) {
                blocks = _region.World.GetVerticalColumn(_position, y - 1);
            }
            else {
                blocks = _region.World.GetVerticalColumn(_position, y - 1, _lightMinHeight[0]);
            }

            var enumerable = blocks as Block[] ?? blocks.ToArray();
            foreach (var block in enumerable) {
                if (block == null) continue;
                var reduction = block.StaticData.SunlightPassReduction;
                if (reduction == 0) {
                    block.BlockLight.LinearSunlight = modifiedLight;
                    continue;
                }

                modifiedLight -= (sbyte) Math.Max(modifiedLight - reduction, 0);

                for (var i = previousLight - 1; i >= modifiedLight; i--) {
                    _lightMinHeight[i] = y;
                }

                previousLight = modifiedLight;
                block.BlockLight.LinearSunlight = modifiedLight;
            }
            
            foreach (var block in enumerable) {
                block?.RemoveSunlight();
            }
            foreach (var block in enumerable) {
                block?.ExpandSunlight();
            }
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