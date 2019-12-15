using System;
using System.Collections.Generic;
using CMineNew.Color;
using CMineNew.Map;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Resources.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Gui {
    public class WorldGui {
        private readonly World _world;
        private readonly Dictionary<string, Gui2dElement> _2dElements;
        private readonly Dictionary<string, GuiBlockElement> _blockElements;
        private readonly ShaderProgram _2dElementsShader, _blockElementsShader;

        private readonly VertexArrayObject _2dElementsVertexArrayObject;
        private readonly VertexArrayObject _blockElementVertexArrayObject;

        public WorldGui(World world) {
            _world = world;
            _2dElements = new Dictionary<string, Gui2dElement>();
            _2dElementsShader = ShaderManager.GetOrCreateShader("2d_elements",
                Shaders.gui_2d_element_vertex, Shaders.gui_2d_element_fragment);
            _2dElementsShader.Use();
            _2dElementsShader.SetUInt("sampler", 0);
            _2dElementsVertexArrayObject = new VertexArrayObject(WorldGuiStaticElements.Vertices,
                WorldGuiStaticElements.Indices);

            _blockElements = new Dictionary<string, GuiBlockElement>();
            _blockElementsShader =
                ShaderManager.GetOrCreateShader("block_elements", Shaders.gui_block_element_vertex,
                    Shaders.gui_block_element_fragment);
            _blockElementVertexArrayObject = BlockFaceVertices.CreateCubeVao();
        }

        public void Add2dElement(Gui2dElement element) {
            _2dElements.Add(element.Name, element);
        }

        public void Remove2dElement(string name) {
            _2dElements.Remove(name);
        }

        public void AddBlockElement(GuiBlockElement element) {
            _blockElements.Add(element.Name, element);
        }

        public void RemoveBlockElement(string name) {
            _blockElements.Remove(name);
        }

        public void Draw() {
            _2dElementsShader.Use();
            _2dElementsVertexArrayObject.Bind();
            foreach (var element in _2dElements.Values) {
                element.Draw(_world, _2dElementsShader, _2dElementsVertexArrayObject);
            }

            var aspect = _world.Camera.Frustum.AspectRatio;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.TextureMap.Texture);

            _blockElementsShader.Use();
            _blockElementVertexArrayObject.Bind();
            _blockElementsShader.SetUMatrix("guiProjection",
                Matrix4.CreatePerspectiveOffCenter(-aspect, aspect, -1, 1, 1, 5));
            
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            
            foreach (var element in _blockElements.Values) {
                element.Draw(_world, _blockElementsShader, _blockElementVertexArrayObject);
            }
        }

        public void GenerateDefault() {
            var hotbarBackground = new Gui2dElement("hotbar", new Vector2(0.3f, -0.9f), new Vector2(0.4f, 0.1f) * 1.5f,
                CMine.TextureManager.GetOrCreateTexture("hotbar", Textures.gui_hotbar),
                AspectRatioMode.PreserveXModifyY);
            Add2dElement(hotbarBackground);


            var hotbar = _world.Player.Inventory.Hotbar;
            for (var i = 0; i < 5; i++) {
                AddBlockElement(new GuiHotbarBlock(i, hotbar, hotbarBackground));
            }
            
        }

        public void CleanUp() {
            _2dElementsVertexArrayObject.CleanUp();
            _blockElementVertexArrayObject.CleanUp();
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