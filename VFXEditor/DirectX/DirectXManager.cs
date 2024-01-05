using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;
using VfxEditor.DirectX.Material;
using VfxEditor.DirectX.Renderers;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class DirectXManager {
        private Device Device;
        private DeviceContext Ctx;

        public readonly ModelPreview ModelPreview;
        public readonly GradientRenderer GradientView;
        public readonly PapPreview PapPreview;
        public readonly BoneNamePreview PhybPreview;
        public readonly BoneNamePreview SklbPreview;
        public readonly BoneNamePreview EidPreview;
        public readonly MaterialPreview MaterialPreview;
        public readonly SsaoPreview MeshPreview;

        private readonly List<ModelRenderer> Renderers = new();

        public static Include IncludeHandler { get; private set; }

        public DirectXManager() {
            var shaderPath = Path.Combine( Plugin.RootLocation, "Shaders" );
            IncludeHandler = new HLSLFileIncludeHandler( shaderPath );
            Device = Dalamud.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;

            ModelPreview = new( Device, Ctx, shaderPath );
            GradientView = new( Device, Ctx, shaderPath );
            PapPreview = new( Device, Ctx, shaderPath );
            PhybPreview = new( Device, Ctx, shaderPath );
            SklbPreview = new( Device, Ctx, shaderPath );
            EidPreview = new( Device, Ctx, shaderPath );
            MaterialPreview = new( Device, Ctx, shaderPath );
            MeshPreview = new( Device, Ctx, shaderPath );

            Renderers = new() {
                ModelPreview,
                PapPreview,
                PhybPreview,
                SklbPreview,
                EidPreview,
                MaterialPreview,
                MeshPreview,
            };
        }

        public void RedrawMaterials() {
            MaterialPreview.Redraw();
            MeshPreview.Redraw();
        }

        public void Redraw() => Renderers.ForEach( x => x.Redraw() );

        public void Dispose() {
            Renderers.ForEach( x => x.Dispose() );
            Renderers.Clear();

            Device = null;
            Ctx = null;
        }

        public static SharpDX.Vector3 ToVec3( System.Numerics.Vector3 v ) => new( v.X, v.Y, v.Z );
    }
}
