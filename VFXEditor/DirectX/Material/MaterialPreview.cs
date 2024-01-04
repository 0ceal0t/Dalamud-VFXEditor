using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.DirectX.Drawable;
using VfxEditor.DirectX.Renderers;
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


    [StructLayout( LayoutKind.Sequential, Size = 0x10 )]
    public struct VSMaterialBuffer {
        public Vector2 Repeat; // 0x00
        public Vector2 Skew; // 0x08
    }

    [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
    public struct LightData {
        public Vector3 Color;
        public float Radius;
        public Vector3 Position;
        public float Falloff;
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

        public float Roughness; // 0x40
        public float Albedo; // 0x44
        public Vector2 _Pad2; // 0x48

        public Vector3 ViewDirection; // 0x50
        public float _Pad3; // 0x5C

        // ----- 0x60 ------

        public LightData Light1;
        public LightData Light2;
    }

    public class MaterialPreview : ModelRenderer {
        private readonly D3dDrawable Model;

        public MtrlFile CurrentFile { get; private set; }
        public MtrlColorTableRow CurrentColorRow { get; private set; }

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer PSBufferData;
        protected Buffer MaterialVertexShaderBuffer;
        protected VSMaterialBuffer VSBufferData;

        protected ShaderResourceView DiffuseView;
        protected ShaderResourceView NormalView;
        protected Texture2D DiffuseTexture;
        protected Texture2D NormalTexture;

        private bool SkipDraw => DiffuseTexture == null || NormalTexture == null || DiffuseTexture.IsDisposed || NormalTexture.IsDisposed;

        public MaterialPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };
            VSBufferData = new() { };

            Model = new( 5, false,
                new InputElement[] {
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "TANGENT", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "BITANGENT", 0, Format.R32G32B32A32_Float, 32, 0 ),
                    new( "UV", 0, Format.R32G32B32A32_Float, 48, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 64, 0 )
                } );
            Model.AddPass( Device, PassType.Final, Path.Combine( shaderPath, "Material.fx" ), ShaderPassFlags.Pixel );

            var builder = new MeshBuilder( true, true, true );
            builder.AddSphere( new Vector3( 0, 0, 0 ), 0.5f, 500, 500 );
            var data = FromMeshBuilder( builder, null, true, true, true, out var count );
            Model.SetVertexes( Device, data, count );
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
                DiffuseColor = DirectXManager.ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Diffuse ) ? CurrentColorRow.DyeData.Diffuse : row.Diffuse.Value ),
                EmissiveColor = DirectXManager.ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Emissive ) ? CurrentColorRow.DyeData.Emissive : row.Emissive.Value ),
                SpecularColor = DirectXManager.ToVec3( applyDye && dyeRow.Flags.HasFlag( DyeRowFlags.Apply_Specular ) ? CurrentColorRow.DyeData.Specular : row.Specular.Value ),
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

        protected override void DrawPasses() {
            if( SkipDraw ) return;

            var psBuffer = PSBufferData with {
                AmbientColor = DirectXManager.ToVec3( Plugin.Configuration.MaterialAmbientColor ),
                Roughness = Plugin.Configuration.MaterialRoughness,
                Albedo = Plugin.Configuration.MaterialAlbedo,
                ViewDirection = CameraPosition,
                Light1 = Plugin.Configuration.Light1.GetData(),
                Light2 = Plugin.Configuration.Light2.GetData(),
            };

            var vsBuffer = VSBufferData;

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );

            Ctx.PixelShader.SetSampler( 0, Sampler );
            Ctx.PixelShader.SetShaderResource( 0, DiffuseView );
            Ctx.PixelShader.SetShaderResource( 1, NormalView );

            Model.Draw(
                Ctx, PassType.Final,
                new List<Buffer>() { VertexShaderBuffer, MaterialVertexShaderBuffer },
                new List<Buffer>() { PixelShaderBuffer, MaterialPixelShaderBuffer } );
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