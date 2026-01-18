using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;
using VfxEditor.DirectX.Mesh;
using VfxEditor.DirectX.Gradient;
using VfxEditor.DirectX.Model;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class DirectXManager {
        public readonly Device Device;
        public readonly DeviceContext Ctx;

        public readonly GradientRenderer GradientRenderer;
        public readonly ModelPreview ModelRenderer;
        public readonly BonePreview<ModelInstance> BoneRenderer;
        public readonly BoneNamePreview BoneNameRenderer;
        public readonly MaterialPreviewLegacy MaterialRenderer;
        public readonly MeshPreview MeshRenderer;

        public readonly List<RenderInstance> Instances = [];
        public readonly List<Renderer> Renderers = [];

        public static Include IncludeHandler { get; private set; }

        public DirectXManager() {
            var shaderPath = Path.Combine( Plugin.RootLocation, "Shaders" );
            IncludeHandler = new HLSLFileIncludeHandler( shaderPath );
            Device = new Device( Dalamud.PluginInterface.UiBuilder.DeviceHandle );
            Ctx = Device.ImmediateContext;

            GradientRenderer = new( Device, Ctx, shaderPath );
            ModelRenderer = new( Device, Ctx, shaderPath );
            BoneRenderer = new( Device, Ctx, shaderPath );
            BoneNameRenderer = new( Device, Ctx, shaderPath );
            MaterialRenderer = new( Device, Ctx, shaderPath );
            MeshRenderer = new( Device, Ctx, shaderPath );

            Renderers.AddRange( [
                GradientRenderer,
                ModelRenderer,
                BoneRenderer,
                BoneNameRenderer,
                MaterialRenderer,
                MeshRenderer
            ] );
        }

        public void Redraw() {
            foreach( var instance in Instances ) instance.NeedsRender = true;
            foreach( var renderer in Renderers ) renderer.NeedsUpdate = true;
        }

        public void Dispose() {
            GradientRenderer.Dispose();
            ModelRenderer.Dispose();
            BoneRenderer.Dispose();
            BoneNameRenderer.Dispose();
            MaterialRenderer.Dispose();
            MeshRenderer.Dispose();

            Instances.Clear();
            Renderers.Clear();
        }
    }
}
