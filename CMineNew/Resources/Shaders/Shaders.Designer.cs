﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMineNew.Resources.Shaders {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Shaders {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Shaders() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CMineNew.Resources.Shaders.Shaders", typeof(Shaders).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos, fragNormal;
        ///in vec2 fragTexCoord;
        ///flat in vec4 fragColorFilter;
        ///in float fragLight;
        ///
        ///layout (location = 0) out vec2 gNormal;
        ///layout (location = 1) out vec3 gAlbedo;
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D sampler;
        ///
        ///void main() {
        ///    vec4 texture4 = texture(sampler, fragTexCoord);
        ///    if (texture4.w &lt; 0.1) discard;
        ///    vec3 texture = texture4.rgb;
        ///
        ///    if (texture.r == texture.g &amp;&amp; texture.r == texture.b &amp;&amp; fragColorFilter.a &gt; 0.5)  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string block_fragment {
            get {
                return ResourceManager.GetString("block_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) out vec2 gNormal;
        ///layout (location = 1) out vec4 gAlbedo;
        ///
        ///void main() {
        ///    gAlbedo = vec4(0, 0, 0, 0);
        ///    //W = Specular Weight
        ///    gNormal = vec2(0, 1);
        ///}.
        /// </summary>
        internal static string block_lines_fragment {
            get {
                return ResourceManager.GetString("block_lines_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///
        ///uniform mat4 viewProjection;
        ///uniform vec3 worldPosition;
        ///
        ///void main () {
        ///    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y, worldPosition.z, 1);
        ///    vec4 modelPosition = model * vec4(position, 1);
        ///    gl_Position = viewProjection * modelPosition;
        ///    gl_Position.z -= 0.0005;
        ///}
        ///.
        /// </summary>
        internal static string block_lines_vertex {
            get {
                return ResourceManager.GetString("block_lines_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///layout (location = 3) in vec3 worldPosition;
        ///layout (location = 4) in float textureIndex;
        ///layout (location = 5) in float blockColorFilter;
        ///layout (location = 6) in float blockLight;
        ///layout (location = 7) in float sunlight;
        ///
        ///out vec3 fragPos, fragNormal;
        ///out vec2 fragTexCoord;
        ///flat out vec4 fragColorFilter;
        ///out float fragLight;
        ///
        ///layout (std140, bindin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string block_vertex {
            get {
                return ResourceManager.GetString("block_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec2 fragPosition, fragTexCoords;
        ///in vec3 fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
        ///
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D gDepth;
        ///uniform sampler2D gNormal;
        ///
        ///uniform mat4 invertedViewProjection;
        ///uniform vec3 cameraPosition;
        ///
        ///void main() {
        ///    vec4 normalFull = texture(gNormal, fragTexCoords);
        ///    vec3 normal = normalFull.rgb;
        ///    
        ///    if(normal.x == 0 &amp;&amp; normal.y == 0 &amp;&amp; normal.z == 0) {
        ///        gBrightness =  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string directional_light_fragment {
            get {
                return ResourceManager.GetString("directional_light_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec2 position;
        ///layout (location = 1) in vec2 texturePosition;
        ///layout (location = 2) in vec3 lightDirection;
        ///layout (location = 3) in vec3 ambientColor;
        ///layout (location = 4) in vec3 diffuseColor;
        ///layout (location = 5) in vec3 specularColor;
        ///
        ///out vec2 fragPosition, fragTexCoords;
        ///out vec3 fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy, 0, 1);
        ///    fragPosition = position.xy; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string directional_light_vertex {
            get {
                return ResourceManager.GetString("directional_light_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///
        ///in vec2 fragTexCoords;
        ///in vec3 fragLightPosition, fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
        ///in float fragConstantAttenuation, fragLinearAttenuation, fragQuadraticAttenuation;
        ///in float fragCutOff, fragOuterCutOff;
        ///
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D gDepth;
        ///uniform sampler2D gNormal;
        ///
        ///uniform vec3 cameraPosition;
        ///
        ///void main() {
        ///
        ///    vec4 normalFull = texture(gNormal, fragTexCoords);
        ///    vec3 normal = normalFull [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string flash_light_fragment {
            get {
                return ResourceManager.GetString("flash_light_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec2 position;
        ///layout (location = 1) in vec2 texturePosition;
        ///layout (location = 2) in vec3 lightPosition;
        ///layout (location = 3) in vec3 lightDirection;
        ///layout (location = 4) in vec3 ambientColor;
        ///layout (location = 5) in vec3 diffuseColor;
        ///layout (location = 6) in vec3 specularColor;
        ///layout (location = 7) in float constantAttenuation;
        ///layout (location = 8) in float linearAttenuation;
        ///layout (location = 9) in float quadraticAttenuation;
        ///layout (locatio [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string flash_light_vertex {
            get {
                return ResourceManager.GetString("flash_light_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos;
        ///in vec2 fragTexCoord;
        ///
        ///out vec4 FragColor;
        ///
        ///uniform sampler2D elementTexture;
        ///
        ///void main() {
        ///    FragColor = texture(elementTexture, fragTexCoord);
        ///    if (FragColor.w &lt; 0.1) discard;
        ///}.
        /// </summary>
        internal static string gui_2d_element_fragment {
            get {
                return ResourceManager.GetString("gui_2d_element_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///
        ///out vec3 fragPos;
        ///out vec2 fragTexCoord;
        ///
        ///uniform vec2 instancePosition;
        ///uniform vec2 instanceSize;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy * instanceSize + instancePosition, 1, 1);
        ///    fragTexCoord = texturePosition;
        ///}
        ///.
        /// </summary>
        internal static string gui_2d_element_vertex {
            get {
                return ResourceManager.GetString("gui_2d_element_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec2 fragTexCoord;
        ///flat in vec4 fragColorFilter;
        ///
        ///out vec4 outColor;
        ///
        ///uniform sampler2D sampler;
        ///
        ///void main() {
        ///    vec4 texture4 = texture(sampler, fragTexCoord);
        ///    if (texture4.w &lt; 0.1) discard;
        ///    vec3 texture = texture4.rgb;
        ///
        ///    if (texture.r == texture.g &amp;&amp; texture.r == texture.b &amp;&amp; fragColorFilter.a &gt; 0.5) {
        ///        texture *= fragColorFilter.rgb;
        ///    }
        ///    
        ///    outColor = vec4(texture, 1);
        ///}.
        /// </summary>
        internal static string gui_block_element_fragment {
            get {
                return ResourceManager.GetString("gui_block_element_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///
        ///out vec2 fragTexCoord;
        ///flat out vec4 fragColorFilter;
        ///
        ///uniform vec2 instancePosition;
        ///uniform vec3 instanceSize;
        ///uniform int[6] instanceTextureIndices;
        ///uniform mat4 model, guiProjection;
        ///
        ///uniform vec4 colorFilter;
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPositi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string gui_block_element_vertex {
            get {
                return ResourceManager.GetString("gui_block_element_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec4 fragColor;
        ///out vec4 FragColor;
        ///
        ///void main() {
        ///    FragColor = fragColor;
        ///}.
        /// </summary>
        internal static string loop_delay_viewer_fragment {
            get {
                return ResourceManager.GetString("loop_delay_viewer_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///
        ///out vec4 fragColor;
        ///
        ///void main () {
        ///    gl_Position = vec4(position, 1);
        ///    if(gl_VertexID % 6 &gt; 3) {
        ///        fragColor = vec4(1, 0, 0, 1);
        ///    }
        ///    else if(gl_VertexID % 6 &gt; 1) {
        ///        fragColor = vec4(0, 1, 0, 1);
        ///    }
        ///    else {
        ///        fragColor = vec4(0, 0, 1, 1);
        ///    }
        ///}
        ///.
        /// </summary>
        internal static string loop_delay_viewer_vertex {
            get {
                return ResourceManager.GetString("loop_delay_viewer_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///struct PointLight {
        ///    vec3 position;// 12
        ///    vec3 ambientColor;// 12
        ///    vec3 diffuseColor;// 12
        ///    vec3 specularColor;// 12
        ///
        ///    float constantAttenuation;// 4
        ///    float linearAttenuation;// 4
        ///    float quadraticAttenuation;// 4
        ///};
        ///
        ///in vec2 fragPosition, fragTexCoords;
        ///
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D gDepth;
        ///uniform sampler2D gNormal;
        ///
        ///
        ///uniform vec3 cameraPosition;
        ///uniform mat4 invertedViewProjection;
        ///uniform int lightAmount;
        ///la [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string point_light_fragment {
            get {
                return ResourceManager.GetString("point_light_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec2 position;
        ///layout (location = 1) in vec2 texturePosition;
        ///
        ///out vec2 fragPosition, fragTexCoords;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy, 0, 1);
        ///    fragPosition = position.xy;
        ///    fragTexCoords = texturePosition;
        ///}
        ///.
        /// </summary>
        internal static string point_light_vertex {
            get {
                return ResourceManager.GetString("point_light_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos;
        ///in vec2 fragTexCoord;
        ///
        ///out vec4 FragColor;
        ///
        ///uniform sampler2D pointer;
        ///
        ///void main() {
        ///    vec4 texture = texture(pointer, fragTexCoord);
        ///    if(texture.w &lt; 0.1) discard;
        ///    FragColor = vec4(1);
        ///}.
        /// </summary>
        internal static string pointer_fragment {
            get {
                return ResourceManager.GetString("pointer_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///
        ///out vec3 fragPos;
        ///out vec2 fragTexCoord;
        ///
        ///uniform float aspectRatio;
        ///
        ///void main () {
        ///    gl_Position = vec4(position, 1);
        ///    gl_Position.y *= aspectRatio;
        ///    fragTexCoord = texturePosition;
        ///}
        ///.
        /// </summary>
        internal static string pointer_vertex {
            get {
                return ResourceManager.GetString("pointer_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec2 fragPos, fragTexCoords;
        ///out vec4 FragColor;
        ///
        ///uniform sampler2D gAlbedo;
        ///uniform sampler2D gDepth;
        ///uniform sampler2D gNormal;
        ///uniform sampler2D gBrightness;
        ///uniform samplerCube skyBox;
        ///
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPosition;
        ///    vec3 sunlightDirection;
        ///    float viewDistanceSquared;
        ///    float viewDistanceOffsetSquared;
        ///    bool waterShader;
        ///    int millis;
        ///    flo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string post_render_fragment {
            get {
                return ResourceManager.GetString("post_render_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec2 position;
        ///layout (location = 1) in vec2 texturePosition;
        ///
        ///out vec2 fragPos, fragTexCoords;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy, 0, 1);
        ///    fragPos = position.xy;
        ///    fragTexCoords = texturePosition;
        ///}
        ///.
        /// </summary>
        internal static string post_render_vertex {
            get {
                return ResourceManager.GetString("post_render_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec2 fragPos, fragTexCoords;
        ///out vec4 FragColor;
        ///
        ///uniform sampler2D gAlbedo;
        ///uniform sampler2D gDepth;
        ///uniform sampler2D gNormal;
        ///uniform sampler2D gBrightness;
        ///uniform samplerCube skyBox;
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPosition;
        ///    vec3 sunlightDirection;
        ///    float viewDistanceSquared;
        ///    float viewDistanceOffsetSquared;
        ///    bool waterShader;
        ///    int millis;
        ///    float [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string post_render_water_fragment {
            get {
                return ResourceManager.GetString("post_render_water_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragTexCoord;
        ///
        ///layout (location = 0) out vec2 gNormal;
        ///layout (location = 1) out vec3 gAlbedo;
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform samplerCube skyBox;
        ///
        ///void main() {
        ///    gAlbedo = texture(skyBox, fragTexCoord).rgb;
        ///    gNormal = vec2(2);
        ///    gBrightness = vec3(1);
        ///}.
        /// </summary>
        internal static string sky_box_fragment {
            get {
                return ResourceManager.GetString("sky_box_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///
        ///out vec3 fragTexCoord;
        ///
        ///uniform mat4 viewProjection;
        ///
        ///void main () {
        ///    gl_Position = viewProjection * vec4(position, 1);
        ///    gl_Position = gl_Position.xyww;
        ///    fragTexCoord = position;
        ///}
        ///.
        /// </summary>
        internal static string sky_box_vertex {
            get {
                return ResourceManager.GetString("sky_box_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos, fragNormal;
        ///in vec2 fragTexCoord;
        ///in vec4 fragColorFilter;
        ///in float fragLight;
        ///
        ///layout (location = 0) out vec2 gNormal;
        ///layout (location = 1) out vec3 gAlbedo;
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D sampler;
        ///
        ///void main() {
        ///    vec4 texture = texture(sampler, fragTexCoord);
        ///    if (texture.w &lt; 0.1) discard;
        ///
        ///    if (texture.r == texture.g &amp;&amp; texture.r == texture.b &amp;&amp; fragColorFilter.a &gt; 0.5) {
        ///        texture = fragColorFilter * te [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string slab_fragment {
            get {
                return ResourceManager.GetString("slab_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///layout (location = 3) in vec3 worldPosition;
        ///layout (location = 4) in float textureIndex;
        ///layout (location = 5) in float blockColorFilter;
        ///layout (location = 6) in float blockLight;
        ///layout (location = 7) in float sunlight;
        ///layout (location = 8) in float upside;
        ///
        ///out vec3 fragPos, fragNormal;
        ///out vec2 fragTexCoord;
        ///out vec4 fragColorFilter;
        ///out float f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string slab_vertex {
            get {
                return ResourceManager.GetString("slab_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec2 fragPos, fragTexCoords;
        ///out float FragColor;
        ///
        ///uniform sampler2D gNormal;
        ///uniform sampler2D gNoise;
        ///
        ///const int kernelSize = 64;
        ///const float radius = 0.5;
        ///const float bias = 0.025;
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPosition;
        ///    vec3 sunlightDirection;
        ///    float viewDistanceSquared;
        ///    float viewDistanceOffsetSquared;
        ///    bool waterShader;
        ///    int millis;
        ///    float nor [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ssao_fragment {
            get {
                return ResourceManager.GetString("ssao_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec2 position;
        ///layout (location = 1) in vec2 texturePosition;
        ///
        ///out vec2 fragPos, fragTexCoords;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy, 0, 1);
        ///    fragPos = position.xy;
        ///    fragTexCoords = texturePosition;
        ///}
        ///.
        /// </summary>
        internal static string ssao_vertex {
            get {
                return ResourceManager.GetString("ssao_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///out vec4 FragColor;
        ///
        ///in vec2 fragTextCoord;
        ///in vec4 fragColor;
        ///
        ///uniform float relative;
        ///uniform sampler2D textureSampler;
        ///
        ///void main() {
        ///    vec4 color = texture(textureSampler, fragTextCoord) * fragColor;
        ///    if(color.w &lt; 0.01) discard;
        ///    FragColor = color;
        ///}.
        /// </summary>
        internal static string static_text_fragment {
            get {
                return ResourceManager.GetString("static_text_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///
        ///layout (location = 3) in vec2 minPosition;
        ///layout (location = 4) in vec2 size;
        ///layout (location = 5) in vec4 color;
        ///layout (location = 6) in vec4 textureCoords;
        ///
        ///out vec2 fragTextCoord;
        ///out vec4 fragColor;
        ///
        ///uniform mat4 model, view, projection;
        ///
        ///void main () {
        ///    gl_Position = vec4(position.xy * size, 0.9f, 1.0) + vec4(minPosition, 0, 0);
        ///    fra [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string static_text_vertex {
            get {
                return ResourceManager.GetString("static_text_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos, fragNormal;
        ///in vec2 fragTexCoord;
        ///flat in vec4 fragColorFilter;
        ///in float fragLight;
        ///
        ///layout (location = 0) out vec2 gNormal;
        ///layout (location = 1) out vec3 gAlbedo;
        ///layout (location = 2) out vec3 gBrightness;
        ///
        ///uniform sampler2D sampler;
        ///
        ///void main() {
        ///    vec4 texture4 = texture(sampler, fragTexCoord);
        ///    if (texture4.w &lt; 0.1) discard;
        ///    vec3 texture = texture4.rgb;
        ///
        ///    if (texture.r == texture.g &amp;&amp; texture.r == texture.b &amp;&amp; fragColorFilter.a &gt; 0.5)  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tall_grass_fragment {
            get {
                return ResourceManager.GetString("tall_grass_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///layout (location = 3) in vec3 worldPosition;
        ///layout (location = 4) in float textureIndex;
        ///layout (location = 5) in float blockColorFilter;
        ///layout (location = 6) in float blockLight;
        ///layout (location = 7) in float sunlight;
        ///
        ///out vec3 fragPos, fragNormal;
        ///out vec2 fragTexCoord;
        ///flat out vec4 fragColorFilter;
        ///out float fragLight;
        ///
        ///layout (std140, bindin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tall_grass_vertex {
            get {
                return ResourceManager.GetString("tall_grass_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos;
        ///in vec2 fragTexCoords;
        ///in vec4 fragColorFilter;
        ///in float fragLight;
        ///
        ///out vec4 FragColor;
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPosition;
        ///    vec3 sunlightDirection;
        ///    float viewDistanceSquared;
        ///    float viewDistanceOffsetSquared;
        ///    bool waterShader;
        ///    int millis;
        ///    float normalizedSpriteSize;
        ///    int spriteTextureLength;
        ///    vec2 windowsSize;
        ///};
        ///
        ///unifo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string water_fragment {
            get {
                return ResourceManager.GetString("water_fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///layout (location = 0) in vec3 position;
        ///layout (location = 1) in vec3 normal;
        ///layout (location = 2) in vec2 texturePosition;
        ///layout (location = 3) in vec3 worldPosition;
        ///layout (location = 4) in float textureIndex;
        ///layout (location = 5) in float blockColorFilter;
        ///layout (location = 6) in float blockLight;
        ///layout (location = 7) in float sunlight;
        ///layout (location = 8) in float waterLevels;
        ///
        ///out vec3 fragPos;
        ///out vec2 fragTexCoords;
        ///out vec4 fragColorFilter;
        ///out float fragLig [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string water_vertex {
            get {
                return ResourceManager.GetString("water_vertex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #version 440 core
        ///
        ///in vec3 fragPos;
        ///in vec2 fragTexCoords;
        ///in vec4 fragColorFilter;
        ///in float fragLight;
        ///
        ///out vec4 FragColor;
        ///
        ///layout (std140, binding = 0) uniform Uniforms {
        ///
        ///    mat4 viewProjection;
        ///    mat4 view;
        ///    mat4 projection;
        ///    vec3 cameraPosition;
        ///    vec3 sunlightDirection;
        ///    float viewDistanceSquared;
        ///    float viewDistanceOffsetSquared;
        ///    bool waterShader;
        ///    int millis;
        ///    float normalizedSpriteSize;
        ///    int spriteTextureLength;
        ///    vec2 windowsSize;
        ///};
        ///
        ///unifo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string water_water_fragment {
            get {
                return ResourceManager.GetString("water_water_fragment", resourceCulture);
            }
        }
    }
}
