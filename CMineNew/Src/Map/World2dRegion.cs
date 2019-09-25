using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.Generator.Biomes;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map{
    public class World2dRegion{
        public const string World2dRegionFolder = "2dRegions";
        public const int RegionChunkLength = 4;
        public const int RegionChunkSize = RegionChunkLength * RegionChunkLength;
        public const int RegionLength = 64;
        public const int RegionSize = RegionLength * RegionLength;
        public const int ChunkPositionShift = 2;
        public const int WorldPositionShift = 6;

        private readonly World _world;
        private readonly Vector2i _position;

        private readonly Biome[,] _biomes;
        private readonly int[,] _heights, _interpolatedHeights;
        private readonly Color4[,] _interpolatedGrassColors;

        private readonly SunlightData[,] _sunlightData;

        public World2dRegion(World world, Vector2i position) {
            _world = world;
            _position = position;
            _biomes = new Biome[RegionLength, RegionLength];
            _heights = new int[RegionLength, RegionLength];
            _interpolatedHeights = new int[RegionLength, RegionLength];
            _interpolatedGrassColors = new Color4[RegionLength, RegionLength];
            _sunlightData = new SunlightData[RegionLength, RegionLength];
        }

        public World World => _world;

        public Vector2i Position => _position;

        public Biome[,] Biomes => _biomes;

        public int[,] Heights => _heights;

        public int[,] InterpolatedHeights => _interpolatedHeights;

        public Color4[,] InterpolatedGrassColors => _interpolatedGrassColors;

        public SunlightData[,] SunlightData => _sunlightData;

        public void CalculateBiomes() {
            var worldPosition = _position << WorldPositionShift;
            var grid = _world.WorldGenerator.BiomeGrid;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    _biomes[x, z] = grid.GetBiome(new Vector2i(x, z) + worldPosition);
                }
            }
        }

        public void CalculateHeightsAndCreateSunlightData() {
            var worldPosition = _position << WorldPositionShift;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    var local = new Vector2i(x, z);
                    _heights[x, z] = _biomes[x, z].GetColumnHeight(worldPosition + local);
                    _sunlightData[x, z] = new SunlightData(this, new Vector2i(x, z));
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
                    _interpolatedHeights[x, z] = height;
                    _interpolatedGrassColors[x, z] = grassColor;
                }
            }
        }

        public void Save() {
            const uint version = 0;
            var regionsFolder = _world.Folder + Path.DirectorySeparatorChar + World2dRegionFolder;
            FolderUtils.CreateRegionFolderIfNotExist(regionsFolder);
            var file = regionsFolder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + ".reg";

            var stream = new DeflateStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write),
                CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, version);

            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    var biome = _biomes[x, z];
                    var height = _heights[x, z];
                    var interpolatedHeight = _interpolatedHeights[x, z];
                    var interpolatedColor = _interpolatedGrassColors[x, z];

                    formatter.Serialize(stream, biome.Id);
                    formatter.Serialize(stream, height);
                    formatter.Serialize(stream, interpolatedHeight);
                    formatter.Serialize(stream, interpolatedColor.R);
                    formatter.Serialize(stream, interpolatedColor.G);
                    formatter.Serialize(stream, interpolatedColor.B);
                    formatter.Serialize(stream, interpolatedColor.A);
                    _sunlightData[x, z].Save(stream, formatter);
                }
            }

            stream.Close();
        }

        public bool Load() {
            var regionsFolder = _world.Folder + Path.DirectorySeparatorChar + World2dRegionFolder;
            FolderUtils.CreateRegionFolderIfNotExist(regionsFolder);
            var file = regionsFolder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + ".reg";
            if (!File.Exists(file)) return false;

            var stream = new DeflateStream(File.Open(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
            var formatter = new BinaryFormatter();
            var version = (uint) formatter.Deserialize(stream);

            var grid = _world.WorldGenerator.BiomeGrid;
            for (var x = 0; x < RegionLength; x++) {
                for (var z = 0; z < RegionLength; z++) {
                    _biomes[x, z] = grid.GetBiomeOrDefault((string) formatter.Deserialize(stream));
                    _heights[x, z] = (int) formatter.Deserialize(stream);
                    _interpolatedHeights[x, z] = (int) formatter.Deserialize(stream);
                    _interpolatedGrassColors[x, z] = new Color4(
                        (float) formatter.Deserialize(stream),
                        (float) formatter.Deserialize(stream),
                        (float) formatter.Deserialize(stream),
                        (float) formatter.Deserialize(stream));

                    var light = new SunlightData(this, new Vector2i(x, z));
                    light.Load(stream, formatter);
                    _sunlightData[x, z] = light;
                }
            }

            stream.Close();

            return true;
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