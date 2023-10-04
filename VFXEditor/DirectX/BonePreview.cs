using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class BonePreview : ModelRenderer {
        protected static readonly int ModelSpan = 3; // position, color, normal

        protected int NumVertices = 0;
        protected Buffer Vertices;

        protected readonly CompilationResult CompiledVS;
        protected readonly CompilationResult CompiledPS;
        protected readonly CompilationResult CompiledGS;
        protected readonly PixelShader PS;
        protected readonly VertexShader VS;
        protected readonly GeometryShader GS;
        protected readonly ShaderSignature Signature;
        protected readonly InputLayout Layout;

        public readonly bool ShaderError = false;

        public BonePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            try {
                var shaderFile = Path.Combine( shaderPath, "ModelPreview.fx" );
                CompiledVS = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
                CompiledPS = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
                CompiledGS = ShaderBytecode.CompileFromFile( shaderFile, "GS", "gs_4_0" );
                VS = new VertexShader( Device, CompiledVS );
                PS = new PixelShader( Device, CompiledPS );
                GS = new GeometryShader( Device, CompiledGS );
                Signature = ShaderSignature.GetInputSignature( CompiledVS );
                Layout = new InputLayout( Device, Signature, new InputElement[] {
                    new("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                    new("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
                } );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error compiling shaders" );
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

            var data = GetData( mesh );
            Vertices?.Dispose();
            Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
            NumVertices = mesh.Indices.Count;
            UpdateDraw();
        }

        protected static Vector4[] GetData( List<MeshGeometry3D> meshes ) {
            var data = new List<Vector4>();
            for( var i = 0; i < meshes.Count; i++ ) {
                data.AddRange( GetData( meshes[i] ) );
            }
            return data.ToArray();
        }

        protected static void PaintColor( MeshGeometry3D mesh, Vector4 color ) {
            var _color = new Color4( color );
            mesh.Colors = new Color4Collection( Enumerable.Repeat( _color, mesh.Positions.Count ).ToArray() );
        }

        protected static Vector4[] GetData( MeshGeometry3D mesh ) {
            var positions = mesh.Positions;
            var normals = mesh.Normals;
            var indexes = mesh.Indices;
            var colors = mesh.Colors;

            var data = new Vector4[indexes.Count * ModelSpan];

            for( var index = 0; index < indexes.Count; index++ ) {
                var pointIdx = indexes[index];
                var position = positions[pointIdx];
                var normal = normals[pointIdx];
                var color = colors[pointIdx];

                var dataIdx = index * ModelSpan;
                data[dataIdx] = new Vector4( position.X, position.Y, position.Z, 1 ); // POSITION
                data[dataIdx + 1] = color.ToVector4();
                data[dataIdx + 2] = new Vector4( normal.X, normal.Y, normal.Z, 0 ); // NORMAL
            }

            return data;
        }

        protected override bool Wireframe() => false;

        protected override bool ShowEdges() => false;

        public override void OnDraw() {
            if( ShaderError ) return;
            if( NumVertices == 0 ) return;

            Ctx.PixelShader.Set( PS );
            Ctx.GeometryShader.Set( GS );
            Ctx.VertexShader.Set( VS );
            Ctx.InputAssembler.InputLayout = Layout;
            Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Ctx.VertexShader.SetConstantBuffer( 0, VSBuffer );
            Ctx.PixelShader.SetConstantBuffer( 0, PSBuffer );
            Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );

            Ctx.Draw( NumVertices, 0 );

            Ctx.GeometryShader.Set( null );
        }

        public override void OnDispose() {
            Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PS?.Dispose();
            VS?.Dispose();
            GS?.Dispose();
            CompiledVS?.Dispose();
            CompiledPS?.Dispose();
            CompiledGS?.Dispose();
        }
    }
}
