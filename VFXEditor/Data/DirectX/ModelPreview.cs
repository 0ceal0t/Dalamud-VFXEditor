using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using System.Reflection;
using AVFXLib.Models;
using Vec2 = System.Numerics.Vector2;

namespace VFXEditor.Data.DirectX {
    public class ModelPreview : Model3D {
        // ======= BASE MODEL =======
        public int NumVerts;
        public Buffer Vertices;
        CompilationResult vertexShaderByteCode;
        CompilationResult pixelShaderByteCode;
        PixelShader PShader;
        VertexShader VShader;
        ShaderSignature Signature;
        InputLayout Layout;

        // ======= EDGES ==========
        public int EDGE_NumVerts;
        public Buffer EDGE_Vertices;
        CompilationResult EDGE_VertexShaderByteCode;
        CompilationResult EDGE_PixelShaderByteCode;
        PixelShader EDGE_PShader;
        VertexShader EDGE_VShader;
        ShaderSignature EDGE_Signature;
        InputLayout EDGE_Layout;

        public ModelPreview( DirectXManager manager ) : base(manager) {
            NumVerts = 0;
            Vertices = null;
            EDGE_NumVerts = 0;
            EDGE_Vertices = null;

            // ======= BASE MODEL =========
            var shaderFile = Path.Combine( Manager.ShaderPath, "ModelPreview.fx" );
            vertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
            pixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
            VShader = new VertexShader( _Device, vertexShaderByteCode );
            PShader = new PixelShader( _Device, pixelShaderByteCode );
            Signature = ShaderSignature.GetInputSignature( vertexShaderByteCode );
            Layout = new InputLayout( _Device, Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
            } );

            // ======= MODEL EDGES ========
            var EDGE_shaderFile = Path.Combine( Manager.ShaderPath, "ModelEdge.fx" );
            EDGE_VertexShaderByteCode = ShaderBytecode.CompileFromFile( EDGE_shaderFile, "VS", "vs_4_0" );
            EDGE_PixelShaderByteCode = ShaderBytecode.CompileFromFile( EDGE_shaderFile, "PS", "ps_4_0" );
            EDGE_VShader = new VertexShader( _Device, EDGE_VertexShaderByteCode );
            EDGE_PShader = new PixelShader( _Device, EDGE_PixelShaderByteCode );
            EDGE_Signature = ShaderSignature.GetInputSignature( EDGE_VertexShaderByteCode );
            EDGE_Layout = new InputLayout( _Device, EDGE_Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            } );
        }

        public static int MODEL_SPAN = 3; // position, color, normal
        public static int EDGE_SPAN = 2;
        public static Vector4 LINE_COLOR = new Vector4( 1, 0, 0, 1 );
        public void LoadModel( AVFXModel model, int mode = 1 ) {
            LoadModel( model.Indexes, model.Vertices, mode );
        }
        public void LoadModel( List<Index> _Indexes, List<Vertex> _Vertices, int mode ) {
            if( _Indexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();

                EDGE_NumVerts = 0;
                EDGE_Vertices?.Dispose();
                return;
            }

            Vector4[] data = new Vector4[_Indexes.Count * 3 * MODEL_SPAN]; // 3 vertices per face
            Vector4[] EDGE_data = new Vector4[_Indexes.Count * 4 * EDGE_SPAN]; // 4 points per loop

            for( int index = 0; index < _Indexes.Count; index++ ) { // each face
                int[] _indexes = new int[] { _Indexes[index].I1, _Indexes[index].I2, _Indexes[index].I3 };
                for( int j = 0; j < _indexes.Length; j++ ) { // push all 3 vertices per face
                    var idx = GetIdx( index, j, MODEL_SPAN, 3 );
                    var v = _Vertices[_indexes[j]];

                    data[idx + 0] = new Vector4( v.Position[0], v.Position[1], v.Position[2], 1.0f );
                    if( mode == 1 ) { // COLOR
                        data[idx + 1] = new Vector4( v.Color[0] / 255, v.Color[1] / 255, v.Color[2] / 255, 1.0f );
                    }
                    else if( mode == 2 ) { // UV1
                        data[idx + 1] = new Vector4( v.UV1[0] + 0.5f, 0, v.UV1[1] + 0.5f, 1.0f );
                    }
                    else if( mode == 3 ) { // UV2
                        data[idx + 1] = new Vector4( v.UV2[2] + 0.5f, 0, v.UV2[3] + 0.5f, 1.0f );
                    }
                    data[idx + 2] = new Vector4( v.Normal[0], v.Normal[1], v.Normal[2], 1.0f );

                    // ========= COPY OVER EDGE DATA ==========
                    int edgeIdx = GetIdx( index, j, EDGE_SPAN, 4 );
                    EDGE_data[edgeIdx + 0] = data[idx + 0];
                    EDGE_data[edgeIdx + 1] = LINE_COLOR;
                    if( j == 0 ) { // loop back around
                        int lastEdgeIdx = GetIdx( index, 3, EDGE_SPAN, 4 );
                        EDGE_data[lastEdgeIdx + 0] = data[idx + 0];
                        EDGE_data[lastEdgeIdx + 1] = LINE_COLOR;
                    }
                }
            }

            Vertices?.Dispose();
            Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, data );
            NumVerts = _Indexes.Count * 3;

            EDGE_Vertices?.Dispose();
            EDGE_Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, EDGE_data );
            EDGE_NumVerts = _Indexes.Count * 4;
        }
        public static int GetIdx( int faceIdx, int pointIdx, int span, int pointsPer ) {
            return span * ( faceIdx * pointsPer + pointIdx );
        }

        public override void OnDraw() {
            // ====== DRAW BASE =========
            _Ctx.PixelShader.Set( PShader );
            _Ctx.VertexShader.Set( VShader );
            _Ctx.InputAssembler.InputLayout = Layout;
            _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
            if( NumVerts > 0 ) {
                _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * MODEL_SPAN, 0 ) ); // set vertex buffer
                _Ctx.Draw( NumVerts, 0 );
            }

            // ====== DRAW LINE =========
            if( ShowEdges ) {
                _Ctx.PixelShader.Set( EDGE_PShader );
                _Ctx.VertexShader.Set( EDGE_VShader );
                _Ctx.InputAssembler.InputLayout = EDGE_Layout;
                _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
                _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                if( EDGE_NumVerts > 0 ) {
                    _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EDGE_Vertices, Utilities.SizeOf<Vector4>() * EDGE_SPAN, 0 ) ); // set vertex buffer
                    for( int i = 0; i < EDGE_NumVerts / 4; i++ ) {
                        _Ctx.Draw( 4, i * 4 );
                    }
                }
            }
        }

        public override void OnDispose() {
            Vertices?.Dispose();
            EDGE_Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            vertexShaderByteCode?.Dispose();
            pixelShaderByteCode?.Dispose();
        }
    }
}
