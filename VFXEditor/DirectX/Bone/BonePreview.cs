using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.DirectX.Drawable;
using VfxEditor.DirectX.Renderers;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class BonePreview : ModelRenderer {
        protected D3dDrawable Model;

        public BonePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            Model = new( 3, false,
                [
                    new("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                    new("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
                ] );
            Model.AddPass( device, PassType.Final, Path.Combine( shaderPath, "Model.fx" ), ShaderPassFlags.Pixel | ShaderPassFlags.Geometry );
        }

        public void LoadSkeleton( BoneSkinnedMeshGeometry3D mesh ) {
            if( mesh.Positions.Count == 0 ) {
                Model.ClearVertexes();
                UpdateDraw();
                return;
            }

            Model.SetVertexes( Device, GetData( mesh ), mesh.Indices.Count );
            UpdateDraw();
        }

        protected static Vector4[] GetData( List<MeshGeometry3D> meshes ) {
            var data = new List<Vector4>();
            for( var i = 0; i < meshes.Count; i++ ) {
                data.AddRange( GetData( meshes[i] ) );
            }
            return [.. data];
        }

        protected static void PaintColor( MeshGeometry3D mesh, Vector4 color ) {
            mesh.Colors = [.. Enumerable.Repeat( new Color4( color ), mesh.Positions.Count ).ToArray()];
        }

        protected static Vector4[] GetData( MeshGeometry3D mesh ) {
            var positions = mesh.Positions;
            var normals = mesh.Normals;
            var indexes = mesh.Indices;
            var colors = mesh.Colors;

            var data = new List<Vector4>();

            for( var index = 0; index < indexes.Count; index++ ) {
                var pointIdx = indexes[index];
                var position = positions[pointIdx];
                var normal = normals[pointIdx];
                var color = colors[pointIdx];

                data.Add( new Vector4( position.X, position.Y, position.Z, 1 ) );
                data.Add( color.ToVector4() );
                data.Add( new Vector4( normal.X, normal.Y, normal.Z, 0 ) );
            }

            return [.. data];
        }

        protected override bool Wireframe() => false;

        protected override bool ShowEdges() => false;

        protected override void DrawPopup() => Plugin.Configuration.DrawDirectXSkeleton();

        protected override void DrawPasses() {
            Model.Draw( Ctx, PassType.Final, VertexShaderBuffer, PixelShaderBuffer );
        }

        public override void Dispose() {
            base.Dispose();
            Model?.Dispose();
        }
    }
}
