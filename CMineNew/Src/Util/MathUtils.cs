using System;

namespace CMineNew.Util {
    public static class MathUtils {
        public static int FloorMod(int x, int n) {
            return (x % n + n) % n;
        }
    }
}