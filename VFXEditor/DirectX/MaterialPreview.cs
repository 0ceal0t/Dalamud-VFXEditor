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
    public struct PSMaterialBuffer {
        public Vector3 CameraPos; // On Draw
        // Init
        // TODO: multiple lights
        public Vector3 LightPos;
        public Vector3 LightDiffuseColor;
        public Vector3 LightSpecularColor;

        // Update with new material
        // TODO: tiles + textures
        public Vector3 DiffuseColor;
        public Vector3 AmbientLightColor; // Init
        public Vector3 EmissiveColor;
        public Vector3 SpecularColor;

        public float SpecularPower;
        public float SpecularIntensity;
        public Vector2 Padding;
    }

    public class MaterialPreview : ModelRenderer {
        private readonly DirectXDrawable Model;

        public MtrlFile CurrentFile { get; private set; }
        public MtrlColorTableRow CurrentColorRow { get; private set; }

        protected Buffer MaterialPixelShaderBuffer;
        protected PSMaterialBuffer BufferData;

        public MaterialPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            MaterialPixelShaderBuffer = new Buffer( Device, Utilities.SizeOf<PSMaterialBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            BufferData = new() { // TODO: update these
                LightPos = new( 1, 1, 1 ),
                LightDiffuseColor = new( 1, 1, 1 ),
                LightSpecularColor = new( 1, 1, 1 ),
                AmbientLightColor = new( 1, 1, 1 ),
            };

            Model = new( Device, Path.Combine( shaderPath, "Material.fx" ), 3, false, false,
                new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "UV", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                } );

            var builder = new MeshBuilder( true, true, true );
            builder.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitX, Vector3.UnitY, 0.25f, 0.5f, true );
            var data = FromMeshBuilder( builder, null, false, false, true, out var count );
            Model.SetVertexes( Device, data, count );
        }

        public void LoadColorRow( MtrlFile file, MtrlColorTableRow row ) {
            CurrentFile = file;
            CurrentColorRow = row;

            var diffuse = row.Diffuse.Value;
            var emissive = row.Emissive.Value;
            var specular = row.Specular.Value;
            var intensity = row.SpecularStrength.Value;
            var power = row.GlossStrength.Value;

            BufferData = BufferData with {
                DiffuseColor = new( diffuse.X, diffuse.Y, diffuse.Z ),
                EmissiveColor = new( emissive.X, emissive.Y, emissive.Z ),
                SpecularColor = new( specular.X, specular.Y, specular.Z ),
                SpecularIntensity = intensity,
                SpecularPower = power,
            };

            // TODO: tiling

            UpdateDraw();
        }

        public void ClearFile() {
            CurrentFile = null;
            CurrentColorRow = null;
        }

        public override void OnDraw() {
            var psBuffer = BufferData with {
                CameraPos = CameraPosition
            };

            Ctx.UpdateSubresource( ref psBuffer, MaterialPixelShaderBuffer );

            Model.Draw( Ctx, new List<Buffer>() { VertexShaderBuffer }, new List<Buffer>() { PixelShaderBuffer, MaterialPixelShaderBuffer } );
        }

        public override void OnDispose() {
            Model?.Dispose();
            MaterialPixelShaderBuffer?.Dispose();
        }

        // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/Resources/Shaders/psCustomMeshBlinnPhong.hlsl
        // https://brooknovak.wordpress.com/2008/11/13/hlsl-per-pixel-point-light-using-phong-blinn-lighting-model/
        // lmMaterial.SpecularColor = lmMaterial.SpecularColor * specularPower;
        // use power to multiply specular, gloss is the actual power constant
        // https://github.com/TexTools/FFXIV_TexTools_UI/blob/8bad2178db77e75830136a04fdc48f257fabb572/FFXIV_TexTools/ViewModels/ColorsetEditorViewModel.cs#L235
    }
}