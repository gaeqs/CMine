using System;
using CMineNew.Geometry;
using CMineNew.Map.Generator.Biomes;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map{
    public class World2dRegion{
        public const int RegionChunkLength = 4;
        public const int RegionChunkSize = RegionChunkLength * RegionChunkLength;
        public const int RegionLength = 64;
        public const int RegionSize = RegionLength * RegionLength;
        public const int ChunkPositionShift = 2;
        public const int WorldPositionShift = 6;

        private readonly World _world;
        private readonly Vector2i _position;

        private readonly Biome[,] _biomes;
        private readonly int[,] _heights, _interpolatedHeihts;
        private readonly Color4[,] _interpolatedGrassColors;

        public World2dRegion(World world, Vector2i position) {
            _world = world;
            _position = position;
            _biomes = new Biome[RegionLength, RegionLength];
            _heights = new int[RegionLength, RegionLength];
            _interpolatedHeihts = new int[RegionLength, RegionLength];
            _interpolatedGrassColors = new Color4[RegionLength, RegionLength];
        }

        public World World => _world;

        public Vector2i Position => _position;

        public Biome[,] Biomes => _biomes;

        public int[,] Heights => _heights;

        public int[,] InterpolatedHeihts => _interpolatedHeihts;

        public Color4[,] InterpolatedGrassColors => _interpolatedGrassColors;

        public void CalculateBiomes() {
            var worldPosition = _position << WorldPositionShift;
            var grid = _world.WorldGenerator.BiomeGrid;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    _biomes[x, z] = grid.GetBiome(new Vector2i(x, z) + worldPosition);
                }
            }
        }

        public void CalculateHeights() {
            var worldPosition = _position << WorldPositionShift;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    var local = new Vector2i(x, z);
                    _heights[x, z] = _biomes[x, z].GetColumnHeight(worldPosition + local);
                }
            }
        }

        public void CalculateInterpolatedHeightsAndColors() {
            var regions = GetNeighbourRegions();
            var worldPosition = _position << WorldPositionShift;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    var local = new Vector2i(x, z);
                    var position = worldPosition + local;
                    Interpolate(regions, position, local, out var height, out var grassColor);
                    _interpolatedHeihts[x, z] = height;
                    _interpolatedGrassColors[x, z] = grassColor;
                }
            }
        }

        private void Interpolate(World2dRegion[,] regions, Vector2i position, Vector2i local, out int height,
            out Color4 grassColor) {
            const int radius = 5;
            height = 0;
            var gColor = Vector3.Zero;
            for (var x = -radius; x <= radius; x++) {
                for (var z = -radius; z <= radius; z++) {
                    var rx = 1;
                    var rz = 1;
                    var relative = position + new Vector2i(x, z);
                    var relativeLocal = local + new Vector2i(x, z);

                    if (relativeLocal.X < 0) {
                        relativeLocal.X += RegionLength;
                        rx--;
                    }
                    else if (relativeLocal.X >= RegionLength) {
                        relativeLocal.X -= RegionLength;
                        rx++;
                    }

                    if (relativeLocal.Y < 0) {
                        relativeLocal.Y += RegionLength;
                        rz--;
                    }
                    else if (relativeLocal.Y >= RegionLength) {
                        relativeLocal.Y -= RegionLength;
                        rz++;
                    }

                    var region = regions[rx, rz];
                    if (region == null) {
                        var biome = _world.WorldGenerator.BiomeGrid.GetBiome(relative);
                        height += biome.GetColumnHeight(relative);
                        var bgColor = biome.GrassColor;
                        gColor += new Vector3(bgColor.R, bgColor.G, bgColor.B);
                    }
                    else {
                        height += region._heights[relativeLocal.X, relativeLocal.Y];
                        var bgColor = region._biomes[relativeLocal.X, relativeLocal.Y].GrassColor;
                        gColor += new Vector3(bgColor.R, bgColor.G, bgColor.B);
                    }
                }
            }

            height /= radius * radius * 4;
            gColor /= radius * radius * 4;
            grassColor = new Color4(gColor.X, gColor.Y, gColor.Z, 1);
        }

        private World2dRegion[,] GetNeighbourRegions() {
            var map = _world.Regions2d;
            var regions = new World2dRegion[3, 3];
            for (var x = 0; x < 3; x++) {
                for (var z = 0; z < 3; z++) {
                    if (x == 1 && z == 1) {
                        regions[x, z] = this;
                    }
                    else {
                        var position = _position - 1 + new Vector2i(x, z);
                        if (map.TryGetValue(position, out var value)) {
                            regions[x, z] = value;
                        }
                    }
                }
            }

            return regions;
        }
    }
}