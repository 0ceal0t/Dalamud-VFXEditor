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
    [StructLayout( LayoutKind.Sequential )]
    public struct Vec3 {
        public float X;
        public float Y;
        public float Z;
        public float Padding; // HLSL weirdness

        public Vec3( float x, float y, float z ) {
            X = x; Y = y; Z = z;
        }

        public Vec3( Vector3 val ) {
            X = val.X;
            Y = val.Y;
            Z = val.Z;
        }

        public Vec3( System.Numerics.Vector3 val ) {
            X = val.X;
            Y = val.Y;
            Z = val.Z;
        }
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PSMaterialBuffer {
        public Vec3 LightDiffuseColor;
        public Vec3 LightSpecularColor;

        // Update with new material
        // TODO: tiles + textures
        public Vec3 DiffuseColor;
        public Vec3 AmbientLightColor;
        public Vec3 EmissiveColor;
        public Vec3 SpecularColor;

        public float SpecularPower;
        public float SpecularIntensity;
        public Vector2 Padding;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct VSMaterialBuffer {
        public Vec3 CameraPos;
        public Vec3 LightPos;
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

            PSBufferData = new() { // TODO: update these
                LightDiffuseColor = new( 0.7f, 0.7f, 0.7f ),
                LightSpecularColor = new( 0.7f, 0.7f, 0.7f ),
                AmbientLightColor = new( 0.2f, 0.2f, 0.2f ),
            };

            VSBufferData = new() { // TODO: update these
                LightPos = new( 1, 1, 1 )
            };

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
                DiffuseColor = new( row.Diffuse.Value ),
                EmissiveColor = new( row.Emissive.Value ),
                SpecularColor = new( row.Specular.Value ),
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
            var psBuffer = PSBufferData;

            var vsBuffer = VSBufferData with {
                CameraPos = new( CameraPosition )
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
    }
}