namespace CMineNew.Resources.Shaders
{
    public static class Shaders
    {
        private static readonly string BasePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Shaders");

        private static string Load(string relativePath)
        {
            var fullPath = Path.Combine(BasePath, relativePath);
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Shader not found at: {fullPath}");
            }

            return File.ReadAllText(fullPath);
        }

        public static string block_vertex => Load("Block/vertex.vert");
        public static string block_fragment => Load("Block/fragment.frag");

        public static string tall_grass_vertex => Load("TallGrass/vertex.vert");
        public static string tall_grass_fragment => Load("TallGrass/fragment.frag");

        public static string slab_vertex => Load("Slab/vertex.vert");
        public static string slab_fragment => Load("Slab/fragment.frag");

        public static string water_vertex => Load("Water/vertex.vert");
        public static string water_fragment => Load("Water/fragment.frag");
        public static string water_water_fragment => Load("Water/water_fragment.frag");

        public static string block_lines_vertex => Load("BlockLines/vertex.vert");
        public static string block_lines_fragment => Load("BlockLines/fragment.frag");

        public static string post_render_vertex => Load("PostRender/vertex.vert");
        public static string post_render_fragment => Load("PostRender/fragment.frag");
        public static string post_render_water_fragment => Load("PostRender/water_fragment.frag");

        public static string ssao_vertex => Load("SSAO/vertex.vert");
        public static string ssao_fragment => Load("SSAO/fragment.frag");

        public static string ssao_blur_vertex => Load("SSAOBlur/vertex.vert");
        public static string ssao_blur_fragment => Load("SSAOBlur/fragment.frag");

        public static string directional_light_vertex => Load("Light/directional_light_vertex.vert");
        public static string directional_light_fragment => Load("Light/directional_light_fragment.frag");

        public static string flash_light_vertex => Load("Light/flash_light_vertex.vert");
        public static string flash_light_fragment => Load("Light/flash_light_fragment.frag");

        public static string point_light_vertex => Load("Light/point_light_vertex.vert");
        public static string point_light_fragment => Load("Light/point_light_fragment.frag");

        public static string static_text_vertex => Load("StaticText/vertex.vert");
        public static string static_text_fragment => Load("StaticText/fragment.frag");

        public static string pointer_vertex => Load("Pointer/vertex.vert");
        public static string pointer_fragment => Load("Pointer/fragment.frag");

        public static string sky_box_vertex => Load("SkyBox/vertex.vert");
        public static string sky_box_fragment => Load("SkyBox/fragment.frag");

        public static string loop_delay_viewer_vertex => Load("LoopDelayViewer/vertex.vert");
        public static string loop_delay_viewer_fragment => Load("LoopDelayViewer/fragment.frag");

        public static string gui_2d_element_vertex => Load("Gui/2dElement/vertex.vert");
        public static string gui_2d_element_fragment => Load("Gui/2dElement/fragment.frag");

        public static string gui_block_element_vertex => Load("Gui/BlockElement/vertex.vert");
        public static string gui_block_element_fragment => Load("Gui/BlockElement/fragment.frag");
    }
}