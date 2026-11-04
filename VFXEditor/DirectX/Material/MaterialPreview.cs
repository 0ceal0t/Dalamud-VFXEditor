using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Data;
using VfxEditor.DirectX.Drawable;
using VfxEditor.DirectX.Model;
using VfxEditor.Formats.MtrlFormat.Data.Color;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Material {
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
        public float Roughness; // 0x4C

        // ----- 0x50 ------

        public LightData Light1;
        public LightData Light2;

        public Matrix4x4 InvViewMatrix;
        public Matrix4x4 InvProjectionMatrix;
    }

    [StructLayout( LayoutKind.Sequential, Size = 0x30 )]
    public struct PSMaterialParamsBuffer {
        public float TileIndex; // 0x00
        public float TileAlpha; // 0x04
        public float SphereIndex; // 0x08
        public float SphereMask; // 0x0C

        public float Metalness; // 0x10
        public float Anisotropy; // 0x14
        public float SheenRate; // 0x18
        public float SheenTint; // 0x1C

        public float SheenAperture; // 0x20
    }


    public class MaterialPreview : ModelDeferredRenderer<ModelDeferredInstance> {
        private readonly D3dDrawable LegacyModel;
        private readonly D3dDrawable Model;

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer PSBufferData;
        protected Buffer MaterialVertexShaderBuffer;
        protected VSMaterialBuffer VSBufferData;
        protected Buffer MaterialParamsBuffer;
        protected PSMaterialParamsBuffer ParamsBufferData;

        // Loaded once, rows only change the layer index
        protected ShaderResourceView TileDiffuseView;
        protected ShaderResourceView TileNormalView;
        protected ShaderResourceView SphereView;
        protected Texture2D TileDiffuseTexture;
        protected Texture2D TileNormalTexture;
        protected Texture2D SphereTexture;

        private bool Legacy = true;
        private bool HasRow = false;

        private bool SkipDraw => !HasRow || TileDiffuseView == null || TileNormalView == null || SphereView == null;

        public MaterialPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialParamsBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<PSMaterialParamsBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };
            VSBufferData = new() { };
            ParamsBufferData = new() { };

            var builder = new MeshBuilder( true, true, true );
            builder.AddSphere( new Vector3( 0, 0, 0 ), 0.5f, 500, 500 );
            var data = FromMeshBuilder( builder, null, true, true, true, out var count );

            // The sphere's poles are along Z, which faces the camera, so rotate them to Y.
            // U also wraps around the full circumference, so double it to keep tiles square like they are on gear
            for( var i = 0; i < data.Length; i += 5 ) {
                foreach( var offset in new[] { 0, 1, 2, 4 } ) {
                    var v = data[i + offset];
                    data[i + offset] = new( v.X, v.Z, -v.Y, v.W );
                }
                data[i + 3] = data[i + 3] with { X = data[i + 3].X * 2 };
            }

            LegacyModel = CreateSphere( Path.Combine( shaderPath, "MaterialLegacyGBuffer.fx" ), data, count );
            Model = CreateSphere( Path.Combine( shaderPath, "MaterialGBuffer.fx" ), data, count );

            // ===== QUAD =========

            Quad.AddPass( Device, PassType.Final, Path.Combine( shaderPath, "SsaoQuad.fx" ), ShaderPassFlags.Pixel );
        }

        private D3dDrawable CreateSphere( string shader, Vector4[] data, int count ) {
            var model = new D3dDrawable( 5, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "TANGENT", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "BITANGENT", 0, Format.R32G32B32A32_Float, 32, 0 ),
                    new( "UV", 0, Format.R32G32B32A32_Float, 48, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 64, 0 )
                ] );
            model.AddPass( Device, PassType.GBuffer, shader, ShaderPassFlags.Pixel );
            model.SetVertexes( Device, data, count );
            return model;
        }

        private void LoadTextures() {
            if( TileDiffuseView != null ) return;

            var diffuse = Plugin.MtrlManager.TileDiffuseFile;
            var normal = Plugin.MtrlManager.TileNormalFile;
            var sphere = Plugin.MtrlManager.SphereFile;
            if( diffuse == null || normal == null || sphere == null ) return;

            TileDiffuseView = GetTextureArray( diffuse.Layers, diffuse.Header.Height, diffuse.Header.Width, out TileDiffuseTexture );
            TileNormalView = GetTextureArray( normal.Layers, normal.Header.Height, normal.Header.Width, out TileNormalTexture );
            SphereView = GetTextureArray( sphere.Layers, sphere.Header.Height, sphere.Header.Width, out SphereTexture );
        }

        public void SetColorRow( int renderId, ModelDeferredInstance instance, MtrlResolvedRow row, bool legacy ) {
            OnUpdate( renderId, instance );
            LoadTextures();

            Legacy = legacy;
            HasRow = true;

            VSBufferData = VSBufferData with {
                Repeat = new( row.TileTransform.X, row.TileTransform.W ),
                Skew = new( row.TileTransform.Y, row.TileTransform.Z ),
            };

            PSBufferData = PSBufferData with {
                DiffuseColor = row.Diffuse,
                EmissiveColor = row.Emissive * row.Exposure,
                SpecularColor = row.Specular,
                SpecularIntensity = 1f,
                Roughness = row.Roughness,
            };

            var tileCount = Plugin.MtrlManager.TileDiffuseFile?.Layers.Count ?? 1;
            var sphereCount = Plugin.MtrlManager.SphereFile?.Layers.Count ?? 1;

            ParamsBufferData = new() {
                TileIndex = Math.Clamp( row.TileIndex, 0, tileCount - 1 ),
                TileAlpha = row.TileAlpha,
                SphereIndex = Math.Clamp( row.SphereIndex, 0, sphereCount - 1 ),
                SphereMask = row.SphereMask,
                Metalness = row.Metalness,
                Anisotropy = row.Anisotropy,
                SheenRate = row.SheenRate,
                SheenTint = row.SheenTint,
                SheenAperture = row.SheenAperture,
            };
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
            var paramsBuffer = ParamsBufferData;

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );
            Ctx.UpdateSubresource( ref paramsBuffer, MaterialParamsBuffer );
        }

        protected override void GBufferPass( ModelDeferredInstance instance ) {
            if( SkipDraw ) return;

            Ctx.PixelShader.SetShaderResource( 0, TileDiffuseView );
            Ctx.PixelShader.SetShaderResource( 1, TileNormalView );
            Ctx.PixelShader.SetShaderResource( 2, SphereView );

            ( Legacy ? LegacyModel : Model ).Draw(
                Ctx, PassType.GBuffer,
                [VertexShaderBuffer, MaterialVertexShaderBuffer],
                [PixelShaderBuffer, MaterialPixelShaderBuffer, MaterialParamsBuffer] );
        }

        protected override void QuadPass( ModelDeferredInstance instance ) {
            Quad.Draw(
                Ctx, PassType.Final,
                    [VertexShaderBuffer, MaterialVertexShaderBuffer],
                    [PixelShaderBuffer, MaterialPixelShaderBuffer] );
        }

        public override void Dispose() {
            base.Dispose();
            LegacyModel?.Dispose();
            Model?.Dispose();
            MaterialVertexShaderBuffer?.Dispose();
            MaterialPixelShaderBuffer?.Dispose();
            MaterialParamsBuffer?.Dispose();

            TileDiffuseView?.Dispose();
            TileDiffuseTexture?.Dispose();
            TileNormalView?.Dispose();
            TileNormalTexture?.Dispose();
            SphereView?.Dispose();
            SphereTexture?.Dispose();
        }
    }
}