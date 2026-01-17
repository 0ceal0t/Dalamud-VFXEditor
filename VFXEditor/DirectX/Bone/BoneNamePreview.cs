using HelixToolkit.SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.DirectX.Drawable;
using VfxEditor.FileManager;
using VfxEditor.SklbFormat;
using VfxEditor.DirectX.Bone;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using BoneStruct = HelixToolkit.SharpDX.Animations.Bone;

namespace VfxEditor.DirectX {
    public class BoneNamePreview : BonePreview<BoneNameInstance> {
        private int NumWireframe = 0;
        private Buffer WireframeVertices;

        public BoneNamePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void SetSkeleton( int renderId, BoneNameInstance instance, FileManagerFile file, List<BoneStruct> boneList, BoneSkinnedMeshGeometry3D mesh ) {
            instance.SetSkeleton( file is SklbFile, boneList );
            SetSkeleton( renderId, instance, mesh );
        }

        public void SetEmpty( int renderId, BoneNameInstance instance, FileManagerFile file ) {
            SetEmpty( renderId, instance );
            instance.SetSkeleton( file is SklbFile, [] );

            NumWireframe = 0;
            WireframeVertices?.Dispose();
        }

        public void SetEmptyWireFrame() {
            NumWireframe = 0;
            WireframeVertices?.Dispose();
        }

        public void SetWireFrame( int renderId, BoneNameInstance instance, MeshGeometry3D collision, MeshGeometry3D simulation, MeshGeometry3D spring ) {
            OnUpdate( renderId, instance );

            PaintColor( collision, new( 0, 1, 0, 1 ) );
            PaintColor( simulation, new( 0, 0, 1, 1 ) );
            PaintColor( spring, new( 1, 0, 0, 1 ) );

            var meshes = new List<MeshGeometry3D>() {
                collision,
                simulation,
                spring
            };

            if( meshes.Select( x => x.Positions.Count ).Sum() == 0 ) {
                NumWireframe = 0;
                WireframeVertices?.Dispose();
                return;
            }

            var data = GetData( meshes );

            WireframeVertices?.Dispose();
            WireframeVertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumWireframe = meshes.Select( x => x.Indices.Count ).Sum();
        }

        protected override void RenderPasses( BoneNameInstance instance ) {
            if( Model.ShaderError ) return;
            if( Model.Count == 0 && NumWireframe == 0 ) return;

            Model.SetupPass( Ctx, PassType.Final );
            Model.SetConstantBuffers( Ctx, VertexShaderBuffer, PixelShaderBuffer );

            if( Model.Count > 0 ) {
                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Model.Data, SharpDX.Utilities.SizeOf<Vector4>() * Model.Span, 0 ) );
                Ctx.Draw( Model.Count, 0 );
                Ctx.Flush();
            }

            Ctx.Flush();

            if( NumWireframe > 0 ) {
                // Kind of jank, but oh well
                var wireframe = new RasterizerState( Device, new RasterizerStateDescription {
                    CullMode = CullMode.None,
                    DepthBias = 0,
                    DepthBiasClamp = 0,
                    FillMode = FillMode.Wireframe,
                    IsAntialiasedLineEnabled = false,
                    IsDepthClipEnabled = true,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = true,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0
                } );

                Ctx.Rasterizer.State = wireframe;

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( WireframeVertices, SharpDX.Utilities.SizeOf<Vector4>() * 3, 0 ) );
                Ctx.Draw( NumWireframe, 0 );
                Ctx.Flush();

                Ctx.Rasterizer.State = RasterizeState;
                wireframe.Dispose();
            }

            Ctx.GeometryShader.Set( null );
        }

        public override void Dispose() {
            base.Dispose();
            WireframeVertices?.Dispose();
        }
    }
}
