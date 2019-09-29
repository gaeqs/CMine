using System;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Map{
    public class WorldShaderData{
        // ViewProjection, Camera Position, Sunlight Direction,
        // View Distance Squared, View Distance Offset Squared, In Water.
        private static readonly UboObject[] Objects = {
            UboObject.Matrix4, UboObject.Vector3, UboObject.Vector3,
            UboObject.Float, UboObject.Float, UboObject.Boolean,
        };

        private UniformBufferObject _ubo;

        public WorldShaderData() {
            _ubo = new UniformBufferObject(Objects);
            _ubo.BindToBase(0);
        }

        public void SetData(Matrix4 vp, Vector3 cp, Vector3 sd, float vds, float vdos, bool ws) {
            _ubo.StartMapping();
            _ubo.Set(vp, 0);
            _ubo.Set(cp, 1);
            _ubo.Set(sd, 2);
            _ubo.Set(vds, 3);
            _ubo.Set(vdos, 4);
            _ubo.Set(ws, 5);
            _ubo.FinishMapping();
        }
    }
}