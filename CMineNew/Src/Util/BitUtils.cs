using OpenTK;
using OpenTK.Mathematics;
using Vector3i = CMineNew.Geometry.Vector3i;

namespace CMineNew.Util{
    public static class BitUtils{
        public static float IntBitsToFloat(int v) {
            unsafe {
                return *(float*) &v;
            }
        }

        public static Vector3 IntBitsToFloat(Vector3i v) {
            return new Vector3(IntBitsToFloat(v.X), IntBitsToFloat(v.Y), IntBitsToFloat(v.Z));
        }
    }
}