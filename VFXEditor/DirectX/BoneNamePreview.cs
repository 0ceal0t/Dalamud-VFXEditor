using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileManager;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    public class BoneNamePreview : BonePreview {
        public FileManagerFile CurrentFile { get; private set; }
        private List<Bone> BoneList;
        private int NumWireframe = 0;
        private Buffer WireframeVertices;

        private static readonly ClosenessComparator Comparator = new();

        public BoneNamePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( FileManagerFile file, List<Bone> boneList, BoneSkinnedMeshGeometry3D mesh ) {
            BoneList = boneList;
            CurrentFile = file;
            LoadSkeleton( mesh );
        }

        public void LoadWireframe( MeshGeometry3D collision, MeshGeometry3D simulation, MeshGeometry3D spring ) {
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
                UpdateDraw();
                return;
            }

            var data = GetData( meshes );

            WireframeVertices?.Dispose();
            WireframeVertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumWireframe = meshes.Select( x => x.Indices.Count ).Sum();
            UpdateDraw();
        }

        public void LoadEmpty( FileManagerFile file ) {
            CurrentFile = file;
            NumVertices = 0;
            NumWireframe = 0;
            BoneList = new();
            Vertices?.Dispose();
            WireframeVertices?.Dispose();
            UpdateDraw();
        }

        public override void OnDraw() {
            if( ShaderError ) return;
            if( NumVertices == 0 && NumWireframe == 0 ) return;

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

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( WireframeVertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                Ctx.Draw( NumWireframe, 0 );
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
            WireframeVertices?.Dispose();
            base.OnDispose();
        }

        public override void DrawInline() {
            if( !Plugin.Configuration.ShowBoneNames ) {
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
