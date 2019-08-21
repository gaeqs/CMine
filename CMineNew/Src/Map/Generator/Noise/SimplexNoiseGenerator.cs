using System;

/**
 * C# implementation of https://hub.spigotmc.org/stash/projects/SPIGOT/repos/bukkit/browse/src/main/java/org/bukkit/util/noise/SimplexNoiseGenerator.java.
 */
namespace CMine.Map.Generator.Noise{
    public class SimplexNoiseGenerator : PerlinNoiseGenerator{
        protected static readonly double Sqrt3 = Math.Sqrt(3);
        protected static readonly double Sqrt5 = Math.Sqrt(5);
        protected static readonly double F2 = 0.5 * (Sqrt3 - 1);
        protected static readonly double G2 = (3 - Sqrt3) / 6;
        protected static readonly double G22 = G2 * 2.0 - 1;
        protected const double F3 = 1.0 / 3.0;
        protected const double G3 = 1.0 / 6.0;
        protected static readonly double F4 = (Sqrt5 - 1.0) / 4.0;
        protected static readonly double G4 = (5.0 - Sqrt5) / 20.0;
        protected static readonly double G42 = G4 * 2.0;
        protected static readonly double G43 = G4 * 3.0;
        protected static readonly double G44 = G4 * 4.0 - 1.0;

        protected static readonly int[][] Grad4 = {
            new[] {0, 1, 1, 1}, new[] {0, 1, 1, -1}, new[] {0, 1, -1, 1}, new[] {0, 1, -1, -1},
            new[] {0, -1, 1, 1}, new[] {0, -1, 1, -1}, new[] {0, -1, -1, 1}, new[] {0, -1, -1, -1},
            new[] {1, 0, 1, 1}, new[] {1, 0, 1, -1}, new[] {1, 0, -1, 1}, new[] {1, 0, -1, -1},
            new[] {-1, 0, 1, 1}, new[] {-1, 0, 1, -1}, new[] {-1, 0, -1, 1}, new[] {-1, 0, -1, -1},
            new[] {1, 1, 0, 1}, new[] {1, 1, 0, -1}, new[] {1, -1, 0, 1}, new[] {1, -1, 0, -1},
            new[] {-1, 1, 0, 1}, new[] {-1, 1, 0, -1}, new[] {-1, -1, 0, 1}, new[] {-1, -1, 0, -1},
            new[] {1, 1, 1, 0}, new[] {1, 1, -1, 0}, new[] {1, -1, 1, 0}, new[] {1, -1, -1, 0},
            new[] {-1, 1, 1, 0}, new[] {-1, 1, -1, 0}, new[] {-1, -1, 1, 0}, new[] {-1, -1, -1, 0}
        };

        protected static readonly int[][] Simplex = {
            new[] {0, 1, 2, 3}, new[] {0, 1, 3, 2}, new[] {0, 0, 0, 0}, new[] {0, 2, 3, 1}, new[] {0, 0, 0, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {1, 2, 3, 0},
            new[] {0, 2, 1, 3}, new[] {0, 0, 0, 0}, new[] {0, 3, 1, 2}, new[] {0, 3, 2, 1}, new[] {0, 0, 0, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {1, 3, 2, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {1, 2, 0, 3}, new[] {0, 0, 0, 0}, new[] {1, 3, 0, 2}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {0, 0, 0, 0}, new[] {2, 3, 0, 1}, new[] {2, 3, 1, 0},
            new[] {1, 0, 2, 3}, new[] {1, 0, 3, 2}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {2, 0, 3, 1}, new[] {0, 0, 0, 0}, new[] {2, 1, 3, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0},
            new[] {2, 0, 1, 3}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {3, 0, 1, 2},
            new[] {3, 0, 2, 1}, new[] {0, 0, 0, 0}, new[] {3, 1, 2, 0},
            new[] {2, 1, 0, 3}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {0, 0, 0, 0}, new[] {3, 1, 0, 2},
            new[] {0, 0, 0, 0}, new[] {3, 2, 0, 1}, new[] {3, 2, 1, 0}
        };

        public static readonly SimplexNoiseGenerator DefaultSimplexNoise = new SimplexNoiseGenerator();

        protected double _offsetW;

        public SimplexNoiseGenerator() {
        }

        public SimplexNoiseGenerator(int seed) : base(seed) {
        }

        public SimplexNoiseGenerator(Random random) : base(random) {
            _offsetW = random.NextDouble() * 256;
        }

        public static double Dot(int[] g, double x, double y) {
            return g[0] * x + g[1] * y;
        }

        public static double Dot(int[] g, double x, double y, double z) {
            return g[0] * x + g[1] * y + g[2] * z;
        }

        public static double Dot(int[] g, double x, double y, double z, double w) {
            return g[0] * x + g[1] * y + g[2] * z + g[3] * w;
        }

        public override double Noise(double x, double y = 0, double z = 0) {
            if (z == 0) {
                return Noise2D(x, y);
            }

            x += _offset.X;
            y += _offset.Y;
            z += _offset.Z;

            double n0, n1, n2, n3; // Noise contributions from the four corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
            var i = Floor(x + s);
            var j = Floor(y + s);
            var k = Floor(z + s);
            var t = (i + j + k) * G3;
            var x0 = i - t; // Unskew the cell origin back to (x,y,z) space
            var y0 = j - t;
            var z0 = k - t;
            var x0C = x - x0; // The x,y,z distances from the cell origin
            var y0C = y - y0;
            var z0C = z - z0;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.

            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            if (x0C >= y0C) {
                if (y0C >= z0C) {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } // X Y Z order
                else if (x0C >= z0C) {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } // X Z Y order
                else {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } // Z X Y order
            }
            else { // x0<y0
                if (y0C < z0C) {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } // Z Y X order
                else if (x0C < z0C) {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } // Y Z X order
                else {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } // Y X Z order
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            var x1 = x0C - i1 + G3; // Offsets for second corner in (x,y,z) coords
            var y1 = y0C - j1 + G3;
            var z1 = z0C - k1 + G3;
            var x2 = x0C - i2 + 2.0 * G3; // Offsets for third corner in (x,y,z) coords
            var y2 = y0C - j2 + 2.0 * G3;
            var z2 = z0C - k2 + 2.0 * G3;
            var x3 = x0C - 1.0 + 3.0 * G3; // Offsets for last corner in (x,y,z) coords
            var y3 = y0C - 1.0 + 3.0 * G3;
            var z3 = z0C - 1.0 + 3.0 * G3;

            // Work out the hashed gradient indices of the four simplex corners
            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var gi0 = _perm[ii + _perm[jj + _perm[kk]]] % 12;
            var gi1 = _perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]] % 12;
            var gi2 = _perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]] % 12;
            var gi3 = _perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]] % 12;

            // Calculate the contribution from the four corners
            var t0 = 0.6 - x0C * x0C - y0C * y0C - z0C * z0C;
            if (t0 < 0) {
                n0 = 0.0;
            }
            else {
                t0 *= t0;
                n0 = t0 * t0 * Dot(Grad3[gi0], x0C, y0C, z0C);
            }

            var t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0) {
                n1 = 0.0;
            }
            else {
                t1 *= t1;
                n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1, z1);
            }

            var t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) {
                n2 = 0.0;
            }
            else {
                t2 *= t2;
                n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2, z2);
            }

            var t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) {
                n3 = 0.0;
            }
            else {
                t3 *= t3;
                n3 = t3 * t3 * Dot(Grad3[gi3], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0 * (n0 + n1 + n2 + n3);
        }

        public double Noise2D(double x, double z) {
            x += _offset.X;
            z += _offset.Y;

            double n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + z) * F2; // Hairy factor for 2D
            var i = Floor(x + s);
            var j = Floor(z + s);
            var t = (i + j) * G2;
            var x0 = i - t; // Unskew the cell origin back to (x,y) space
            var y0 = j - t;
            var x0C = x - x0; // The x,y distances from the cell origin
            var y0C = z - y0;

            // For the 2D case, the simplex shape is an equilateral triangle.

            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0C > y0C) {
                i1 = 1;
                j1 = 0;
            } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else {
                i1 = 0;
                j1 = 1;
            } // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6

            var x1 = x0C - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            var y1 = y0C - j1 + G2;
            var x2 = x0C + G22; // Offsets for last corner in (x,y) unskewed coords
            var y2 = y0C + G22;

            // Work out the hashed gradient indices of the three simplex corners
            var ii = i & 255;
            var jj = j & 255;
            var gi0 = _perm[ii + _perm[jj]] % 12;
            var gi1 = _perm[ii + i1 + _perm[jj + j1]] % 12;
            var gi2 = _perm[ii + 1 + _perm[jj + 1]] % 12;

            // Calculate the contribution from the three corners
            var t0 = 0.5 - x0C * x0C - y0C * y0C;
            if (t0 < 0) {
                n0 = 0.0;
            }
            else {
                t0 *= t0;
                n0 = t0 * t0 * Dot(Grad3[gi0], x0C, y0C); // (x,y) of grad3 used for 2D gradient
            }

            double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 < 0) {
                n1 = 0.0;
            }
            else {
                t1 *= t1;
                n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1);
            }

            var t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 < 0) {
                n2 = 0.0;
            }
            else {
                t2 *= t2;
                n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 70.0 * (n0 + n1 + n2);
        }

        public double Noise(double x, double y, double z, double w) {
            x += _offset.X;
            y += _offset.Y;
            z += _offset.Z;
            w += _offsetW;

            double n0, n1, n2, n3, n4; // Noise contributions from the five corners

            // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            var s = (x + y + z + w) * F4; // Factor for 4D skewing
            var i = Floor(x + s);
            var j = Floor(y + s);
            var k = Floor(z + s);
            var l = Floor(w + s);

            var t = (i + j + k + l) * G4; // Factor for 4D unskewing
            var x0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            var y0 = j - t;
            var z0 = k - t;
            var w0 = l - t;
            var x0C = x - x0; // The x,y,z,w distances from the cell origin
            var y0C = y - y0;
            var z0C = z - z0;
            var w0C = w - w0;

            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we’re in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            var c1 = (x0C > y0C) ? 32 : 0;
            var c2 = (x0C > z0C) ? 16 : 0;
            var c3 = (y0C > z0C) ? 8 : 0;
            var c4 = (x0C > w0C) ? 4 : 0;
            var c5 = (y0C > w0C) ? 2 : 0;
            var c6 = (z0C > w0C) ? 1 : 0;
            var c = c1 + c2 + c3 + c4 + c5 + c6;
            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner

            // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
            // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
            // impossible. Only the 24 indices which have non-zero entries make any sense.
            // We use a thresholding to set the coordinates in turn from the largest magnitude.

            // The number 3 in the "simplex" array is at the position of the largest coordinate.
            i1 = Simplex[c][0] >= 3 ? 1 : 0;
            j1 = Simplex[c][1] >= 3 ? 1 : 0;
            k1 = Simplex[c][2] >= 3 ? 1 : 0;
            l1 = Simplex[c][3] >= 3 ? 1 : 0;

            // The number 2 in the "simplex" array is at the second largest coordinate.
            i2 = Simplex[c][0] >= 2 ? 1 : 0;
            j2 = Simplex[c][1] >= 2 ? 1 : 0;
            k2 = Simplex[c][2] >= 2 ? 1 : 0;
            l2 = Simplex[c][3] >= 2 ? 1 : 0;

            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i3 = Simplex[c][0] >= 1 ? 1 : 0;
            j3 = Simplex[c][1] >= 1 ? 1 : 0;
            k3 = Simplex[c][2] >= 1 ? 1 : 0;
            l3 = Simplex[c][3] >= 1 ? 1 : 0;

            // The fifth corner has all coordinate offsets = 1, so no need to look that up.

            var x1 = x0C - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            var y1 = y0C - j1 + G4;
            var z1 = z0C - k1 + G4;
            var w1 = w0C - l1 + G4;

            var x2 = x0C - i2 + G42; // Offsets for third corner in (x,y,z,w) coords
            var y2 = y0C - j2 + G42;
            var z2 = z0C - k2 + G42;
            var w2 = w0C - l2 + G42;

            var x3 = x0C - i3 + G43; // Offsets for fourth corner in (x,y,z,w) coords
            var y3 = y0C - j3 + G43;
            var z3 = z0C - k3 + G43;
            var w3 = w0C - l3 + G43;

            var x4 = x0C + G44; // Offsets for last corner in (x,y,z,w) coords
            var y4 = y0C + G44;
            var z4 = z0C + G44;
            var w4 = w0C + G44;

            // Work out the hashed gradient indices of the five simplex corners
            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var ll = l & 255;

            var gi0 = _perm[ii + _perm[jj + _perm[kk + _perm[ll]]]] % 32;
            var gi1 = _perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1 + _perm[ll + l1]]]] % 32;
            var gi2 = _perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2 + _perm[ll + l2]]]] % 32;
            var gi3 = _perm[ii + i3 + _perm[jj + j3 + _perm[kk + k3 + _perm[ll + l3]]]] % 32;
            var gi4 = _perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1 + _perm[ll + 1]]]] % 32;

            // Calculate the contribution from the five corners
            var t0 = 0.6 - x0C * x0C - y0C * y0C - z0C * z0C - w0C * w0C;
            if (t0 < 0) {
                n0 = 0.0;
            }
            else {
                t0 *= t0;
                n0 = t0 * t0 * Dot(Grad4[gi0], x0C, y0C, z0C, w0C);
            }

            var t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0) {
                n1 = 0.0;
            }
            else {
                t1 *= t1;
                n1 = t1 * t1 * Dot(Grad4[gi1], x1, y1, z1, w1);
            }

            var t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0) {
                n2 = 0.0;
            }
            else {
                t2 *= t2;
                n2 = t2 * t2 * Dot(Grad4[gi2], x2, y2, z2, w2);
            }

            var t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0) {
                n3 = 0.0;
            }
            else {
                t3 *= t3;
                n3 = t3 * t3 * Dot(Grad4[gi3], x3, y3, z3, w3);
            }

            var t4 = 0.6 - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0) {
                n4 = 0.0;
            }
            else {
                t4 *= t4;
                n4 = t4 * t4 * Dot(Grad4[gi4], x4, y4, z4, w4);
            }

            // Sum up and scale the result to cover the range [-1,1]
            return 27.0 * (n0 + n1 + n2 + n3 + n4);
        }
    }
}