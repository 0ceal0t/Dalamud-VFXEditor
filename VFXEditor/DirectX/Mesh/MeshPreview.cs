using HelixToolkit.Maths;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.IO;
using VfxEditor.DirectX.Drawable;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.DirectX.Model;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Mesh {
    public class MeshPreview : ModelDeferredRenderer<ModelDeferredInstance> {
        private readonly D3dDrawable Model;

        private readonly HashSet<Buffer> ToCleanUp = [];

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer PSBufferData;
        protected Buffer MaterialVertexShaderBuffer;
        protected VSMaterialBuffer VSBufferData;

        public MeshPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };
            VSBufferData = new() { };

            Model = new( 5, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "TANGENT", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "UV", 0, Format.R32G32B32A32_Float, 32, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 48, 0 ),
                    new( "COLOR", 0, Format.R32G32B32A32_Float, 64, 0 )
                ] );
            Model.AddPass( Device, PassType.GBuffer, Path.Combine( shaderPath, "MeshGBuffer.fx" ), ShaderPassFlags.Pixel );

            // ===== QUAD =========

            Quad.AddPass( Device, PassType.Final, Path.Combine( shaderPath, "SsaoQuad.fx" ), ShaderPassFlags.Pixel );
        }

        public void SetMesh( int renderId, ModelDeferredInstance instance, MdlMeshDrawable mesh ) {
            OnUpdate( renderId, instance );

            if( mesh == null ) return;
            var buffer = mesh.GetBuffer( Device );
            Model.SetVertexes( buffer, ( int )mesh.GetIndexCount() );
            ToCleanUp.Add( buffer );
        }

        protected override void OnRenderUpdate( ModelDeferredInstance instance ) {
            var psBuffer = PSBufferData with {
                AmbientColor = Plugin.Configuration.MaterialAmbientColor,
                EyePosition = instance.CameraPosition,
                Light1 = Plugin.Configuration.Light1.GetData(),
                Light2 = Plugin.Configuration.Light2.GetData(),
                InvViewMatrix = instance.ViewMatrix.Inverted(),
                InvProjectionMatrix = instance.ProjMatrix.Inverted(),

                DiffuseColor = new( 1f, 1f, 1f ),
                EmissiveColor = new( 0f, 0f, 0f ),
                SpecularColor = new( 1f, 1f, 1f ),
                SpecularIntensity = 1f,
                SpecularPower = 1f,
            };

            var vsBuffer = VSBufferData with {
                Repeat = new( 1, 1 ),
                Skew = new( 0, 0 )
            };

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );
        }

        protected override void GBufferPass( ModelDeferredInstance instance ) {
            Model.Draw(
                Ctx, PassType.GBuffer,
                [VertexShaderBuffer, MaterialVertexShaderBuffer],
                [PixelShaderBuffer, MaterialPixelShaderBuffer] );
        }

        protected override void QuadPass( ModelDeferredInstance instance ) {
            Quad.Draw(
                Ctx, PassType.Final,
                    [VertexShaderBuffer, MaterialVertexShaderBuffer],
                    [PixelShaderBuffer, MaterialPixelShaderBuffer] );
        }

        public override void Dispose() {
            base.Dispose();
            Model?.Dispose();
            MaterialVertexShaderBuffer?.Dispose();
            MaterialPixelShaderBuffer?.Dispose();

            foreach( var buffer in ToCleanUp ) buffer?.Dispose();
            ToCleanUp.Clear();
        }
    }
}