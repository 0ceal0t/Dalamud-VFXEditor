using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.PhybFormat;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    public class PhybPreview : AnimationPreview {
        public PhybFile CurrentFile { get; private set; }
        private List<Bone> BoneList;
        private int NumPhysics = 0;
        private Buffer PhysicsVertices;

        private static readonly ClosenessComparator Comparator = new();

        public PhybPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PhybFile file, List<Bone> boneList, BoneSkinnedMeshGeometry3D mesh ) {
            BoneList = boneList;
            CurrentFile = file;
            LoadSkeleton( mesh );
        }

        public void LoadPhysics( MeshGeometry3D collision, MeshGeometry3D simulation, MeshGeometry3D spring ) {
            var meshes = new List<MeshGeometry3D>() {
                collision,
                simulation,
                spring
            };
            var colors = new List<Vector4>() {
                new Vector4( 0, 1, 0, 1 ),
                new Vector4( 0, 0, 1, 1 ),
                new Vector4( 1, 0, 0, 1 ),
            };

            if( meshes.Select( x => x.Positions.Count ).Sum() == 0 ) {
                NumPhysics = 0;
                PhysicsVertices?.Dispose();
                UpdateDraw();
                return;
            }

            var data = GetData( meshes, colors );

            PhysicsVertices?.Dispose();
            PhysicsVertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumPhysics = meshes.Select( x => x.Indices.Count ).Sum();
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

            Ctx.PixelShader.Set( PS );
            Ctx.GeometryShader.Set( GS );
            Ctx.VertexShader.Set( VS );
            Ctx.InputAssembler.InputLayout = Layout;
            Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Ctx.VertexShader.SetConstantBuffer( 0, VSBuffer );
            Ctx.PixelShader.SetConstantBuffer( 0, PSBuffer );

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

            Ctx.GeometryShader.Set( null );
        }

        public void ClearFile() {
            CurrentFile = null;
        }

        public override void OnDispose() {
            PhysicsVertices?.Dispose();
            base.OnDispose();
        }

        public override void DrawInline() {
            if( !Plugin.Configuration.PhybShowBoneName ) {
                base.DrawInline();
                return;
            }

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = LocalMatrix * viewProj;

            using var child = ImRaii.Child( "3DChild" );

            var drawList = ImGui.GetWindowDrawList();

            DrawImage( out var topLeft, out var bottomRight );
            var size = bottomRight - topLeft;
            var mid = topLeft + ( size / 2f );

            var boneScreenPositions = new Dictionary<string, Vec2>();

            foreach( var bone in BoneList ) {
                var matrix = bone.BindPose * worldViewProj;

                var pos = Vector3.Transform( new Vector3( 0 ), matrix ).ToVector3();
                var screenPos = mid + ( ( size / 2f ) * new Vec2( pos.X, -1f * pos.Y ) );
                boneScreenPositions[bone.Name] = screenPos;
            }

            var groups = boneScreenPositions.GroupBy( entry => entry.Value, entry => entry.Key, Comparator );
            foreach( var group in groups ) {
                var pos = group.Key;

                var idx = 0;
                foreach( var item in group ) {
                    drawList.AddText( pos + new Vec2( 0, 12f * idx ), 0xFFFFFFFF, item );
                    idx++;
                }
            }
        }

        private class ClosenessComparator : IEqualityComparer<Vec2> {
            public bool Equals( Vec2 x, Vec2 y ) => ( x - y ).Length() < 10f;
            public int GetHashCode( Vec2 obj ) => 0;
        }
    }
}
