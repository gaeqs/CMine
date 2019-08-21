using System;

namespace CMine.Map.Generator.Noise{
    public class PerlinNoiseGenerator : NoiseGenerator{
        private static readonly int[] P = {
            151, 160, 137, 91, 90, 15, 131, 13, 201,
            95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
            240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
            94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
            87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
            48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
            230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
            63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
            196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
            64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
            82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
            101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
            113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
            193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
            14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
            176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
            114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        protected static readonly int[][] Grad3 = {
            new[] {1, 1, 0}, new[] {-1, 1, 0}, new[] {1, -1, 0}, new[] {-1, -1, 0},
            new[] {1, 0, 1}, new[] {-1, 0, 1}, new[] {1, 0, -1}, new[] {-1, 0, -1},
            new[] {0, 1, 1}, new[] {0, -1, 1}, new[] {0, 1, -1}, new[] {0, -1, -1}
        };

        public static readonly PerlinNoiseGenerator DefaultPerlinNoise = new PerlinNoiseGenerator();


        public PerlinNoiseGenerator() {
            for (var i = 0; i < 512; i++) {
                _perm[i] = P[i & 255];
            }
        }

        public PerlinNoiseGenerator(int seed) : this(new Random(seed)) {
        }

        public PerlinNoiseGenerator(Random random) {
            _offset.X = random.NextDouble() * 256;
            _offset.Y = random.NextDouble() * 256;
            _offset.Z = random.NextDouble() * 256;

            for (var i = 0; i < 256; i++) {
                _perm[i] = random.Next(256);
            }

            for (var i = 0; i < 256; i++) {
                var pos = random.Next(256 - i) + i;
                var aux = _perm[i];
                _perm[i] = _perm[pos];
                _perm[pos] = aux;
                _perm[i + 256] = _perm[i];
            }
        }

        public override double Noise(double x, double y = 0, double z = 0) {
            x += _offset.X;
            y += _offset.Y;
            z += _offset.Z;

            var floorX = Floor(x);
            var floorY = Floor(y);
            var floorZ = Floor(z);

            var ax = floorX & 255;
            var ay = floorY & 255;
            var az = floorY & 255;

            x -= floorX;
            y -= floorY;
            z -= floorZ;

            var fX = Fade(x);
            var fY = Fade(y);
            var fZ = Fade(z);

            var a = _perm[ax] + ay;
            var aa = _perm[a] + az;
            var ab = _perm[a + 1] + az;
            var b = _perm[ax + 1] + ay;
            var ba = _perm[b] + az;
            var bb = _perm[b + 1] + az;

            return Lerp(fZ,
                Lerp(fY, Lerp(fX, Grad(_perm[aa], x, y, z), Grad(_perm[ba], x - 1, y, z)),
                    Lerp(fX, Grad(_perm[ab], x, y - 1, z), Grad(_perm[bb], x - 1, y - 1, z))),
                Lerp(fY, Lerp(fX, Grad(_perm[aa + 1], x, y, z - 1), Grad(_perm[ba + 1], x - 1, y, z - 1)),
                    Lerp(fX, Grad(_perm[ab + 1], x, y - 1, z - 1), Grad(_perm[bb + 1], x - 1, y - 1, z - 1))));
        }
    }
}