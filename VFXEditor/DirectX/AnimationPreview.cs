using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.IO;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class AnimationPreview : ModelRenderer {
        private static readonly int ModelSpan = 3; // position, color, normal
        private int NumVerts;
        private Buffer Vertices;
        private readonly CompilationResult VertexShaderByteCode;
        private readonly CompilationResult PixelShaderByteCode;
        private readonly PixelShader PShader;
        private readonly VertexShader VShader;
        private readonly ShaderSignature Signature;
        private readonly InputLayout Layout;

        public AnimationPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            NumVerts = 0;
            Vertices = null;
            var shaderFile = Path.Combine( shaderPath, "ModelPreview.fx" );
            VertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
            PixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
            VShader = new VertexShader( Device, VertexShaderByteCode );
            PShader = new PixelShader( Device, PixelShaderByteCode );
            Signature = ShaderSignature.GetInputSignature( VertexShaderByteCode );
            Layout = new InputLayout( Device, Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
            } );
        }

        public void LoadAnimation( BoneSkinnedMeshGeometry3D boneMesh ) {
            if( boneMesh.Positions.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();
                return;
            }

            var positions = boneMesh.Positions;
            var normals = boneMesh.Normals;
            var indexes = boneMesh.Indices;

            var data = new Vector4[indexes.Count * ModelSpan];

            for( var index = 0; index < indexes.Count; index++ ) {
                var pointIdx = indexes[index];
                var position = positions[pointIdx];
                var normal = normals[pointIdx];

                var dataIdx = index * ModelSpan;
                data[dataIdx] = new Vector4( position.X, position.Y, position.Z, 1 ); // POSITION
                data[dataIdx + 1] = new Vector4( 1, 1, 1, 1 ); // COLOR
                data[dataIdx + 2] = new Vector4( normal.X, normal.Y, normal.Z, 0 ); // NORMAL
            }

            Vertices?.Dispose();
            Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumVerts = indexes.Count;

            UpdateDraw();
        }

        public override void OnDraw() {
            if( NumVerts > 0 ) {
                Ctx.PixelShader.Set( PShader );
                Ctx.VertexShader.Set( VShader );
                Ctx.InputAssembler.InputLayout = Layout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                Ctx.Draw( NumVerts, 0 );
            }
        }

        public override void OnDispose() {
            Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            VertexShaderByteCode?.Dispose();
            PixelShaderByteCode?.Dispose();
        }
    }
}
