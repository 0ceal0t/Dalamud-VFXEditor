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

    [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
    public struct VSMaterialBuffer {
        public Vector3 ViewDirection; // 0x00
        public float _Pad0;

        public Vector3 LightPos; // 0x10
        public float _Pad1;
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

        public MaterialPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            MaterialVertexShaderBuffer = new Buffer( Device, Utilities.SizeOf<VSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            PSBufferData = new() { };

            VSBufferData = new() { };

            Model = new( Device, Path.Combine( shaderPath, "Material.fx" ), 3, false, false,
                new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "UV", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                } );

            var builder = new MeshBuilder( true, true, true );
            builder.AddSphere( new Vector3( 0, 0, 0 ), 0.5f, 500, 500 );
            var data = FromMeshBuilder( builder, null, false, false, true, out var count );
            Model.SetVertexes( Device, data, count );
        }

        public void LoadColorRow( MtrlFile file, MtrlColorTableRow row ) {
            CurrentFile = file;
            CurrentColorRow = row;

            PSBufferData = PSBufferData with {
                DiffuseColor = ToVec3( row.Diffuse.Value ),
                EmissiveColor = ToVec3( row.Emissive.Value ),
                SpecularColor = ToVec3( row.Specular.Value ),
                SpecularIntensity = row.SpecularStrength.Value,
                SpecularPower = row.GlossStrength.Value,
            };

            // TODO: tiling

            UpdateDraw();
        }

        public void ClearFile() {
            CurrentFile = null;
            CurrentColorRow = null;
        }

        public override void OnDraw() {
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
                ViewDirection = ViewDirection
            };

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );
            Ctx.UpdateSubresource( ref vsBuffer, MaterialVertexShaderBuffer );

            Model.Draw( Ctx, new List<Buffer>() { VertexShaderBuffer, MaterialVertexShaderBuffer }, new List<Buffer>() { PixelShaderBuffer, MaterialPixelShaderBuffer } );
        }

        public override void OnDispose() {
            Model?.Dispose();
            MaterialVertexShaderBuffer?.Dispose();
            MaterialPixelShaderBuffer?.Dispose();
        }

        // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/Resources/Shaders/psCustomMeshBlinnPhong.hlsl
        // https://brooknovak.wordpress.com/2008/11/13/hlsl-per-pixel-point-light-using-phong-blinn-lighting-model/
        // lmMaterial.SpecularColor = lmMaterial.SpecularColor * specularPower;
        // use power to multiply specular, gloss is the actual power constant
        // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/ViewModels/ColorsetEditorViewModel.cs#L235

        public static Vector3 ToVec3( System.Numerics.Vector3 v ) => new( v.X, v.Y, v.Z );
    }
}