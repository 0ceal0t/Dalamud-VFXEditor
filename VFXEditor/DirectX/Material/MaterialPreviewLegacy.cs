using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.DirectX.Drawable;
using VfxEditor.Formats.MtrlFormat.Data.Color;
using VfxEditor.Formats.MtrlFormat.Data.Dye;
using VfxEditor.DirectX.Model;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-packing-rules
    // "Additionally, HLSL packs data so that it does not cross a 16-byte boundary"
    // I want to die

    // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/Resources/Shaders/psCustomMeshBlinnPhong.hlsl
    // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/ViewModels/ColorsetEditorViewModel.cs#L235
    // https://github.com/stackgl/glsl-lighting-walkthrough/blob/master/lib/shaders/phong.frag

    [StructLayout( LayoutKind.Sequential, Size = 0x10 )]
    public struct VSMaterialBuffer {
        public Vector2 Repeat; // 0x00
        public Vector2 Skew; // 0x08
    }

    [StructLayout( LayoutKind.Sequential, Size = 0xA0 )]
    public struct PSMaterialBuffer {
        public Vector3 DiffuseColor; // 0x00
        public float _Pad0;

        public Vector3 EmissiveColor; // 0x10
        public float _Pad1;

        public Vector3 SpecularColor; // 0x20
        public float SpecularPower; // 0x2C

        public float SpecularIntensity; // 0x30
        public Vector3 AmbientColor; // 0x34

        public Vector3 EyePosition; // 0x40
        public float _Pad3; // 0x4C

        // ----- 0x50 ------

        public LightData Light1;
        public LightData Light2;

        public Matrix4x4 InvViewMatrix;
        public Matrix4x4 InvProjectionMatrix;
    }

    public class MaterialPreviewLegacy : ModelDeferredRenderer<ModelDeferredInstance> {
        private readonly D3dDrawable Model;

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer PSBufferData;
        protected Buffer MaterialVertexShaderBuffer;
        protected VSMaterialBuffer VSBufferData;

        protected ShaderResourceView DiffuseView;
        protected ShaderResourceView NormalView;
        protected Texture2D DiffuseTexture;
        protected Texture2D NormalTexture;

        private bool SkipDraw => DiffuseTexture == null || NormalTexture == null || DiffuseTexture.IsDisposed || NormalTexture.IsDisposed;

        public MaterialPreviewLegacy( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };
            VSBufferData = new() { };

            Model = new( 5, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "TANGENT", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "BITANGENT", 0, Format.R32G32B32A32_Float, 32, 0 ),
                    new( "UV", 0, Format.R32G32B32A32_Float, 48, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 64, 0 )
                ] );
            Model.AddPass( Device, PassType.GBuffer, Path.Combine( shaderPath, "MaterialLegacyGBuffer.fx" ), ShaderPassFlags.Pixel );

            var builder = new MeshBuilder( true, true, true );
            builder.AddSphere( new Vector3( 0, 0, 0 ), 0.5f, 500, 500 );
            var data = FromMeshBuilder( builder, null, true, true, true, out var count );
            Model.SetVertexes( Device, data, count );

            // ===== QUAD =========

            Quad.AddPass( Device, PassType.Final, Path.Combine( shaderPath, "SsaoQuad.fx" ), ShaderPassFlags.Pixel );
        }

        public void SetColorRow( int renderId, ModelDeferredInstance instance, MtrlColorRowLegacy row ) {
            OnUpdate( renderId, instance );

            if( row == null ) return;

            VSBufferData = VSBufferData with {
                Repeat = new( row.TileRepeatX.Value, row.TileRepeatY.Value ),
                Skew = new( row.TileSkew.Value.X, row.TileSkew.Value.Y ),
            };

            var applyDye = row.StainTemplate != null;
            var dyeRow = row.DyeRow;

            PSBufferData = PSBufferData with {
                DiffuseColor = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags_Legacy.Diffuse ) ? row.StainTemplate.Diffuse : row.Diffuse.Value,
                EmissiveColor = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags_Legacy.Emissive ) ? row.StainTemplate.Emissive : row.Emissive.Value,
                SpecularColor = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags_Legacy.Specular ) ? row.StainTemplate.Specular : row.Specular.Value,
                SpecularIntensity = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags_Legacy.Specular_Strength ) ? row.StainTemplate.Power : row.SpecularStrength.Value,
                SpecularPower = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags_Legacy.Gloss ) ? row.StainTemplate.Gloss : row.GlossStrength.Value,
            };

            // Clear out the old
            DiffuseView?.Dispose();
            DiffuseTexture?.Dispose();
            NormalView?.Dispose();
            NormalTexture?.Dispose();

            var tileIdx = row.TileMaterial.Value;
            var diffuse = Plugin.MtrlManager.TileDiffuseFile;
            var normal = Plugin.MtrlManager.TileNormalFile;

            DiffuseView = GetTexture( diffuse.Layers[tileIdx], diffuse.Header.Height, diffuse.Header.Width, out DiffuseTexture );
            NormalView = GetTexture( normal.Layers[tileIdx], normal.Header.Height, normal.Header.Width, out NormalTexture );
        }

        protected override void OnRenderUpdate( ModelDeferredInstance instance ) {
            if( SkipDraw ) return;

            var psBuffer = PSBufferData with {
                AmbientColor = Plugin.Configuration.MaterialAmbientColor,
                EyePosition = instance.CameraPosition,
                Light1 = Plugin.Configuration.Light1.GetData(),
                Light2 = Plugin.Configuration.Light2.GetData(),
                InvViewMatrix = instance.ViewMatrix.Inverted(),
                InvProjectionMatrix = instance.ProjMatrix.Inverted()
            };

            var vsBuffer = VSBufferData;

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );
        }

        protected override void GBufferPass( ModelDeferredInstance instance ) {
            Ctx.PixelShader.SetShaderResource( 0, DiffuseView );
            Ctx.PixelShader.SetShaderResource( 1, NormalView );

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

            DiffuseView?.Dispose();
            DiffuseTexture?.Dispose();
            NormalView?.Dispose();
            NormalTexture?.Dispose();
        }
    }
}