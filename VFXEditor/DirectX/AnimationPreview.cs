using Dalamud.Logging;
using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class AnimationPreview : ModelRenderer {
        protected static readonly int ModelSpan = 3; // position, color, normal

        protected int NumVertices = 0;
        protected Buffer Vertices;

        protected readonly CompilationResult VertexShaderByteCode;
        protected readonly CompilationResult PixelShaderByteCode;
        protected readonly PixelShader PShader;
        protected readonly VertexShader VShader;
        protected readonly ShaderSignature Signature;
        protected readonly InputLayout Layout;

        public readonly bool ShaderError = false;

        public AnimationPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            try {
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
            catch( Exception e ) {
                PluginLog.Error( "Error compiling shaders", e );
                ShaderError = true;
            }
        }

        public void LoadSkeleton( BoneSkinnedMeshGeometry3D mesh ) {
            if( mesh.Positions.Count == 0 ) {
                NumVertices = 0;
                Vertices?.Dispose();
                UpdateDraw();
                return;
            }

            var data = GetData( mesh, new Vector4( 1, 1, 1, 1 ) );
            Vertices?.Dispose();
            Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumVertices = mesh.Indices.Count;
            UpdateDraw();
        }

        protected static Vector4[] GetData( List<MeshGeometry3D> meshes, List<Vector4> colors ) {
            var data = new List<Vector4>();
            for( var i = 0; i < meshes.Count; i++ ) {
                data.AddRange( GetData( meshes[i], colors[i] ) );
            }
            return data.ToArray();
        }

        protected static Vector4[] GetData( MeshGeometry3D mesh, Vector4 color ) {
            var positions = mesh.Positions;
            var normals = mesh.Normals;
            var indexes = mesh.Indices;

            var data = new Vector4[indexes.Count * ModelSpan];

            for( var index = 0; index < indexes.Count; index++ ) {
                var pointIdx = indexes[index];
                var position = positions[pointIdx];
                var normal = normals[pointIdx];

                var dataIdx = index * ModelSpan;
                data[dataIdx] = new Vector4( position.X, position.Y, position.Z, 1 ); // POSITION
                data[dataIdx + 1] = color; // COLOR
                data[dataIdx + 2] = new Vector4( normal.X, normal.Y, normal.Z, 0 ); // NORMAL
            }

            return data;
        }

        public override void OnDraw() {
            if( ShaderError ) return;
            if( NumVertices == 0 ) return;

            Ctx.PixelShader.Set( PShader );
            Ctx.VertexShader.Set( VShader );
            Ctx.InputAssembler.InputLayout = Layout;
            Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
            Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
            Ctx.Draw( NumVertices, 0 );
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
