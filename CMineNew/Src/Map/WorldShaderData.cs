using CMineNew.Geometry;
using CMineNew.Render;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Map{
    public class WorldShaderData{
        // ViewProjection, Camera Position, Sunlight Direction,
        // View Distance Squared, View Distance Offset Squared, In Water.
        private static readonly UboObject[] Objects = {
            UboObject.Matrix4, UboObject.Matrix4, UboObject.Matrix4,
            UboObject.Vector3, UboObject.Vector3, UboObject.Vector3,
            UboObject.Vector3,
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


        /*
         * layout (std140, binding = 0) uniform Uniforms {
           
               mat4 viewProjection;
               mat4 view;
               mat4 projection;
           
               vec3 cameraPosition;
               ivec3 floorCameraPosition;
               vec3 decimalCameraPosition;
           
               vec3 sunlightDirection;
               float viewDistanceSquared;
               float viewDistanceOffsetSquared;
               bool waterShader;
               int millis;
               float normalizedSpriteSize;
               int spriteTextureLength;
               vec2 windowsSize;
           };
         */

        public void SetData(Camera camera, Vector3 sd, float vds, float vdos, bool ws,
            long ticks, float normalizedSpriteSize, int spriteTextureLength, Vector2i windowSize) {
            var cameraPos = camera.Position;
            var cameraPosI = new Vector3i((int) cameraPos.X, (int) cameraPos.Y, (int) cameraPos.Z);
            var cameraPosF = new Vector3((float) cameraPos.X, (float) cameraPos.Y, (float) cameraPos.Z);

            var offset = cameraPos - cameraPosI.ToDouble();
            var offsetF = new Vector3((float) offset.X, (float) offset.Y, (float) offset.Z);

            _millis += (int) (ticks * 1000 / CMine.TicksPerSecond);
            _millis %= 1000000;
            _ubo.Orphan();
            _ubo.Set(camera.Matrix * camera.Frustum.Matrix, 0);
            _ubo.Set(camera.Matrix, 1);
            _ubo.Set(camera.Frustum.Matrix, 2);

            _ubo.Set(cameraPosF, 3);
            _ubo.Set(cameraPosI, 4);
            _ubo.Set(offsetF, 5);


            _ubo.Set(sd, 6);
            _ubo.Set(vds, 7);
            _ubo.Set(vdos, 8);
            _ubo.Set(ws, 9);
            _ubo.Set(_millis, 10);
            _ubo.Set(normalizedSpriteSize, 11);
            _ubo.Set(spriteTextureLength, 12);
            _ubo.Set(windowSize.ToFloat(), 13);
        }
    }
}