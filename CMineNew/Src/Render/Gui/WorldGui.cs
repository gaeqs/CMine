using System.Collections.Generic;
using CMineNew.Map;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Resources.Textures;
using OpenTK;

namespace CMineNew.Render.Gui {
    public class WorldGui {

        private readonly World _world;
        private readonly Dictionary<string, Gui2dElement> _2dElements;
        private readonly ShaderProgram _2dElementsShader;
        private readonly VertexArrayObject _2dElementsVertexArrayObject;

        public WorldGui(World world) {
            _world = world;
            _2dElements = new Dictionary<string, Gui2dElement>();
            _2dElementsShader = ShaderManager.GetOrCreateShader("2d_elements",
                Shaders.gui_2d_element_vertex, Shaders.gui_2d_element_fragment);
            _2dElementsShader.Use();
            _2dElementsShader.SetUInt("sampler", 0);
            _2dElementsVertexArrayObject = new VertexArrayObject(WorldGuiStaticElements.Vertices,
                WorldGuiStaticElements.Indices);
        }

        public void Add2dElement(Gui2dElement element) {
            _2dElements.Add(element.Name, element);
        }

        public void Remove2dElement(string name) {
            _2dElements.Remove(name);
        }

        public void Draw() {
            _2dElementsShader.Use();
            _2dElementsVertexArrayObject.Bind();
            foreach (var element in _2dElements.Values) {
                element.Draw(_world, _2dElementsShader, _2dElementsVertexArrayObject);
            }
        }

        public void GenerateDefault() {
            Add2dElement(new Gui2dElement("hotbar", new Vector2(0.3f, -0.9f) , new Vector2(0.4f, 0.1f) * 1.5f,
                CMine.TextureManager.GetOrCreateTexture("hotbar", Textures.gui_hotbar), AspectRatioMode.PreserveXModifyY));
        }
    }

    static class WorldGuiStaticElements {
        public static readonly Vertex[] Vertices = {
            new Vertex(new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new Vertex(new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new Vertex(new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new Vertex(new Vector3(1, 1, 1), new Vector3(0, 0, 1), new Vector2(1, 0))
        };

        public static readonly int[] Indices = {0, 1, 3, 0, 3, 2};
    }
}