using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using VfxEditor.PhybFormat;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class PhybPreview : AnimationPreview {
        public PhybFile CurrentFile { get; private set; }
        private int NumPhysics = 0;
        private Buffer PhysicsVertices;

        public PhybPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PhybFile file, BoneSkinnedMeshGeometry3D mesh ) {
            CurrentFile = file;
            LoadSkeleton( mesh );
        }

        public void LoadPhysics( MeshGeometry3D mesh ) {
            if( mesh.Positions.Count == 0 ) {
                NumPhysics = 0;
                PhysicsVertices?.Dispose();
                UpdateDraw();
                return;
            }

            var data = GetData( mesh, new Vector4( 0, 1, 0, 1 ) );
            PhysicsVertices?.Dispose();
            PhysicsVertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumPhysics = mesh.Indices.Count;
            UpdateDraw();
        }

        public void LoadEmpty( PhybFile file ) {
            CurrentFile = file;
            NumVertices = 0;
            NumPhysics = 0;
            Vertices?.Dispose();
            PhysicsVertices?.Dispose();
            UpdateDraw();
        }

        public override void OnDraw() {
            if( ShaderError ) return;
            if( NumVertices == 0 && NumPhysics == 0 ) return;

            Ctx.PixelShader.Set( PShader );
            Ctx.VertexShader.Set( VShader );
            Ctx.InputAssembler.InputLayout = Layout;
            Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );

            if( NumVertices > 0 ) {
                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                Ctx.Draw( NumVertices, 0 );
                Ctx.Flush();
            }

            if( NumPhysics > 0 ) {
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

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( PhysicsVertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                Ctx.Draw( NumPhysics, 0 );
                Ctx.Flush();

                Ctx.Rasterizer.State = RasterizeState;
                wireframe.Dispose();
            }
        }

        public void ClearFile() {
            CurrentFile = null;
        }

        public override void OnDispose() {
            PhysicsVertices?.Dispose();
            base.OnDispose();
        }
    }
}
