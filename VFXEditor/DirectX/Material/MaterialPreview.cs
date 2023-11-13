using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.DirectX.Drawable;
using VfxEditor.Formats.MtrlFormat;
using VfxEditor.Formats.MtrlFormat.Table;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-packing-rules
    // "Additionally, HLSL packs data so that it does not cross a 16-byte boundary"
    // I want to die

    // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/Resources/Shaders/psCustomMeshBlinnPhong.hlsl
    // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/ViewModels/ColorsetEditorViewModel.cs#L235
    // https://github.com/stackgl/glsl-lighting-walkthrough/blob/master/lib/shaders/phong.frag

    // TODO: multiple lights

    [StructLayout( LayoutKind.Sequential, Size = 0x30 )]
    public struct VSMaterialBuffer {
        public Vector3 ViewDirection; // 0x00
        public float _Pad0;

        public Vector3 LightPos; // 0x10
        public float _Pad1;

        public Vector2 Repeat; // 0x20
        public Vector2 Skew; // 0x28
    }

    [StructLayout( LayoutKind.Sequential, Size = 0x60 )]
    public struct PSMaterialBuffer {
        public Vector3 DiffuseColor; // 0x00
        public float _Pad0;

        public Vector3 EmissiveColor; // 0x10
        public float _Pad1;

        public Vector3 SpecularColor; // 0x20

        public float SpecularPower; // 0x2C
        public float SpecularIntensity; // 0x30

        public Vector3 LightColor; // 0x34

        public Vector3 AmbientColor; // 0x40

        public float Roughness; // 0x4C
        public float Albedo; // 0x50
        public float Radius; // 0x54
        public float Falloff; // 0x58
    }

    public class MaterialPreview : ModelRenderer {
        private readonly DirectXDrawable Model;

        public MtrlFile CurrentFile { get; private set; }
        public MtrlColorTableRow CurrentColorRow { get; private set; }

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer PSBufferData;
        protected Buffer MaterialVertexShaderBuffer;
        protected VSMaterialBuffer VSBufferData;

        protected SamplerState Sampler;
        protected ShaderResourceView DiffuseView;
        protected ShaderResourceView NormalView;
        protected Texture2D DiffuseTexture;
        protected Texture2D NormalTexture;

        public MaterialPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };

            VSBufferData = new() { };

            Model = new( Device, Path.Combine( shaderPath, "Material.fx" ), 5, false, false,
                new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "TANGENT", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "BITANGENT", 0, Format.R32G32B32A32_Float, 32, 0 ),
                    new InputElement( "UV", 0, Format.R32G32B32A32_Float, 48, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 64, 0 )
                } );

            var builder = new MeshBuilder( true, true, true );
            builder.AddSphere( new Vector3( 0, 0, 0 ), 0.5f, 500, 500 );
            var data = FromMeshBuilder( builder, null, true, true, true, out var count );
            Model.SetVertexes( Device, data, count );

            // ================

            Sampler = new( Device, new() {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            } );
        }

        public void RefreshColorRow() => LoadColorRow( CurrentFile, CurrentColorRow );

        public void LoadColorRow( MtrlFile file, MtrlColorTableRow row ) {
            CurrentFile = file;
            CurrentColorRow = row;

            if( CurrentColorRow == null ) return;

            VSBufferData = VSBufferData with {
                Repeat = new( row.MaterialRepeatX.Value, row.MaterialRepeatY.Value ),
                Skew = new( row.MaterialSkew.Value.X, row.MaterialSkew.Value.Y ),
            };

            var applyDye = CurrentColorRow.DyeData != null;
            var dyeRow = CurrentColorRow.DyeRow;

            PSBufferData = PSBufferData with {
                DiffuseColor = ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Diffuse ) ? CurrentColorRow.DyeData.Diffuse : row.Diffuse.Value ),
                EmissiveColor = ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Emissive ) ? CurrentColorRow.DyeData.Emissive : row.Emissive.Value ),
                SpecularColor = ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Specular ) ? CurrentColorRow.DyeData.Specular : row.Specular.Value ),
                SpecularIntensity = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Specular_Strength ) ? CurrentColorRow.DyeData.Power : row.SpecularStrength.Value,
                SpecularPower = applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Gloss ) ? CurrentColorRow.DyeData.Gloss : row.GlossStrength.Value,
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

            UpdateDraw();
        }

        public void ClearFile() {
            CurrentFile = null;
            CurrentColorRow = null;
        }

        public override void OnDraw() {
            if( DiffuseTexture == null || NormalTexture == null || DiffuseTexture.IsDisposed || NormalTexture.IsDisposed ) return;

            var psBuffer = PSBufferData with {
                LightColor = ToVec3( Plugin.Configuration.MaterialLightColor ),
                AmbientColor = ToVec3( Plugin.Configuration.MaterialAmbientColor ),
                Roughness = Plugin.Configuration.MaterialRoughness,
                Albedo = Plugin.Configuration.MaterialAlbedo,
                Radius = Plugin.Configuration.MaterialLightRadius,
                Falloff = Plugin.Configuration.MaterialLightFalloff
            };

            var vsBuffer = VSBufferData with {
                LightPos = ToVec3( Plugin.Configuration.MaterialLightPosition ),
                ViewDirection = CameraPosition
            };

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );

            Ctx.PixelShader.SetSampler( 0, Sampler );
            Ctx.PixelShader.SetShaderResource( 0, DiffuseView );
            Ctx.PixelShader.SetShaderResource( 1, NormalView );

            Model.Draw( Ctx, new List<Buffer>() { VertexShaderBuffer, MaterialVertexShaderBuffer }, new List<Buffer>() { PixelShaderBuffer, MaterialPixelShaderBuffer } );
        }

        private ShaderResourceView GetTexture( byte[] data, int height, int width, out Texture2D texture ) {
            var stream = DataStream.Create( data, true, true );
            var rect = new DataRectangle( stream.DataPointer, width * 4 );
            texture = new( Device, new() {
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription( 1, 0 ),
            }, rect );

            return new ShaderResourceView( Device, texture );
        }

        public override void OnDispose() {
            Model?.Dispose();
            MaterialVertexShaderBuffer?.Dispose();
            MaterialPixelShaderBuffer?.Dispose();

            Sampler?.Dispose();
            DiffuseView?.Dispose();
            DiffuseTexture?.Dispose();
            NormalView?.Dispose();
            NormalTexture?.Dispose();
        }

        public static Vector3 ToVec3( System.Numerics.Vector3 v ) => new( v.X, v.Y, v.Z );
    }
}