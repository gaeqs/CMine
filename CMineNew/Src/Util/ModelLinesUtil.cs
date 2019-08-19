using System;
using System.Collections.Generic;
using OpenTK;

namespace CMineNew.Util{
    public static class ModelLinesUtil{
        private const float Tolerance = 0.0001f;

        public static int[] CalculateLinesIndices(IReadOnlyList<Vector3> vertices) {
            var list = new List<int>();
            for (var i = 0; i < vertices.Count; i++) {
                for (var j = i + 1; j < vertices.Count; j++) {
                    var a = vertices[i];
                    var b = vertices[j];
                    var ba = a - b;
                    var length = ba.LengthSquared;
                    if (!(Math.Abs(length - ba.X * ba.X) < Tolerance) &&
                        !(Math.Abs(length - ba.Y * ba.Y) < Tolerance) &&
                        !(Math.Abs(length - ba.Z * ba.Z) < Tolerance)) continue;
                    list.Add(i);
                    list.Add(j);
                }
            }

            return list.ToArray();
        }
    }
}