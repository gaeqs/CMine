using CMineNew.Geometry;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Map{
    public class WorldShaderData{
        // ViewProjection, Camera Position, Sunlight Direction,
        // View Distance Squared, View Distance Offset Squared, In Water.
        private static readonly UboObject[] Objects = {
            UboObject.Matrix4, UboObject.Matrix4, UboObject.Matrix4, UboObject.Vector3, UboObject.Vector3,
            UboObject.Float, UboObject.Float, UboObject.Boolean, UboObject.Integer,
            UboObject.Float, UboObject.Integer, UboObject.Vector2, 
        };

        private readonly UniformBufferObject _ubo;
        private int _millis;

        public WorldShaderData() {
            _ubo = new UniformBufferObject(Objects);
            _ubo.BindToBase(0);
            _millis = 0;
        }

        public void SetData(Matrix4 view, Matrix4 proj, Vector3 cp, Vector3 sd, float vds, float vdos, bool ws,
            long ticks, float normalizedSpriteSize, int spriteTextureLength, Vector2i windowSize) {
            _millis += (int) (ticks * 1000 / CMine.TicksPerSecond);
            _millis %= 1000000;
            _ubo.Orphan();
            _ubo.Set(view * proj, 0);
            _ubo.Set(view, 1);
            _ubo.Set(proj, 2);
            _ubo.Set(cp, 3);
            _ubo.Set(sd, 4);
            _ubo.Set(vds, 5);
            _ubo.Set(vdos, 6);
            _ubo.Set(ws, 7);
            _ubo.Set(_millis, 8);
            _ubo.Set(normalizedSpriteSize, 9);
            _ubo.Set(spriteTextureLength, 10);
            _ubo.Set(windowSize.ToFloat(), 11);
        }
    }
}