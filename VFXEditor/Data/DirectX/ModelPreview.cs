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
using AVFXLib.Models;

namespace VFXEditor.Data.DirectX {
    public class ModelPreview : Model3D {
        public bool ShowEdges = true;
        public bool ShowEmitter = true;

        // ======= BASE MODEL =======
        static int MODEL_SPAN = 3; // position, color, normal
        int NumVerts;
        Buffer Vertices;
        CompilationResult VertexShaderByteCode;
        CompilationResult PixelShaderByteCode;
        PixelShader PShader;
        VertexShader VShader;
        ShaderSignature Signature;
        InputLayout Layout;

        // ======= EDGES ==========
        static int EDGE_SPAN = 2;
        int EDGE_NumVerts;
        Buffer EDGE_Vertices;
        CompilationResult EDGE_VertexShaderByteCode;
        CompilationResult EDGE_PixelShaderByteCode;
        PixelShader EDGE_PShader;
        VertexShader EDGE_VShader;
        ShaderSignature EDGE_Signature;
        InputLayout EDGE_Layout;

        // ======= EMITTERS ==========
        static int EMIT_SPAN = 2; // position, normal
        int EMIT_INSTANCE_SPAN = 1; // position
        int EMIT_NumVerts;
        int EMIT_NumInstances;
        Buffer EMIT_Vertices;
        Buffer EMIT_Instances;
        CompilationResult EMIT_VertexShaderByteCode;
        CompilationResult EMIT_PixelShaderByteCode;
        PixelShader EMIT_PShader;
        VertexShader EMIT_VShader;
        ShaderSignature EMIT_Signature;
        InputLayout EMIT_Layout;

        public ModelPreview( DirectXManager manager ) : base( manager ) {
            // ======= BASE MODEL =========
            NumVerts = 0;
            Vertices = null;
            var shaderFile = Path.Combine( Manager.ShaderPath, "ModelPreview.fx" );
            VertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
            PixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
            VShader = new VertexShader( _Device, VertexShaderByteCode );
            PShader = new PixelShader( _Device, PixelShaderByteCode );
            Signature = ShaderSignature.GetInputSignature( VertexShaderByteCode );
            Layout = new InputLayout( _Device, Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
            } );

            // ======= MODEL EDGES ========
            EDGE_NumVerts = 0;
            EDGE_Vertices = null;
            EDGE_VertexShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "ModelEdge_VS.fx" ), "VS", "vs_4_0" );
            EDGE_PixelShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "ModelEdge_PS.fx" ), "PS", "ps_4_0" );
            EDGE_VShader = new VertexShader( _Device, EDGE_VertexShaderByteCode );
            EDGE_PShader = new PixelShader( _Device, EDGE_PixelShaderByteCode );
            EDGE_Signature = ShaderSignature.GetInputSignature( EDGE_VertexShaderByteCode );
            EDGE_Layout = new InputLayout( _Device, EDGE_Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            } );


            // ======= EMITTER VERTICES ========
            // .... It's just a cube ...
            var baseVertices = new[]
            {
                new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),  // TLB 0
                new Vector4(1.0f, 1.0f, -1.0f, 1.0f),   // TRB 1
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),    // TRF 2
                new Vector4(-1.0f, 1.0f, 1.0f, 1.0f),   // TLF 3
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), // BLB 4
                new Vector4(1.0f, -1.0f, -1.0f, 1.0f),  // BRB 5
                new Vector4(1.0f, -1.0f, 1.0f, 1.0f),   // BRF 6
                new Vector4(-1.0f, -1.0f, 1.0f, 1.0f)   // BLF 7
            };
            var faces = new[]
            {
                new int[] { 3, 2, 6, 7 }, // Front
                new int[] { 1, 0, 4, 5 }, // Back
                new int[] { 0, 3, 7, 4 }, // Left
                new int[] { 2, 1, 5, 6 }, // Right
                new int[] { 0, 1, 2, 3 }, // Top
                new int[] { 7, 6, 5, 4 }, // Bottom
            };
            var normals = new[]
            {
                new Vector4(0, 0, 1, 1), // Front
                new Vector4(0, 0, -1, 1), // Back
                new Vector4(-1, 0, 0, 1), // Left
                new Vector4(1, 0, 0, 1), // Right
                new Vector4(0, 1, 0, 1), // Top
                new Vector4(0, -1, 0, 1), // Bottom
            };

            var v_emitter = new Vector4[faces.Length * 6 * EMIT_SPAN]; // 6 VERTICES PER FACE (2 TRIANGLES) * 2 VECTORS PER VERTEX
            for( int i = 0; i < faces.Length; i++ ) {
                var indexes = new int[]{
                    faces[i][0],
                    faces[i][1],
                    faces[i][2],
                    faces[i][0],
                    faces[i][2],
                    faces[i][3]
                };
                for( int j = 0; j < indexes.Length; j++ ) {
                    var idx = i * ( 6 * EMIT_SPAN ) + j * EMIT_SPAN;
                    v_emitter[idx] = baseVertices[indexes[j]] * new Vector4(0.1f, 0.1f, 0.1f, 1.0f); // vertex position, scale it down
                    v_emitter[idx + 1] = normals[i]; // face normal
                }
            }
            EMIT_NumVerts = v_emitter.Length / EMIT_SPAN;
            EMIT_Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, v_emitter );

            EMIT_NumInstances = 0;
            EMIT_Instances = null;
            EMIT_VertexShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "EmitterVertex_VS.fx" ), "VS", "vs_4_0" );
            EMIT_PixelShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "EmitterVertex_PS.fx" ), "PS", "ps_4_0" );
            EMIT_VShader = new VertexShader( _Device, EMIT_VertexShaderByteCode );
            EMIT_PShader = new PixelShader( _Device, EMIT_PixelShaderByteCode );
            EMIT_Signature = ShaderSignature.GetInputSignature( EMIT_VertexShaderByteCode );
            EMIT_Layout = new InputLayout( _Device, EMIT_Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0, InputClassification.PerVertexData, 0),
                new InputElement("INSTANCE", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1)
            } );
        }

        public static Vector4 LINE_COLOR = new Vector4( 1, 0, 0, 1 );
        public void LoadModel( AVFXModel model, int mode = 1 ) {
            LoadModel( model.Indexes, model.Vertices, model.EmitVertices, mode );
        }
        public void LoadModel( List<Index> _Indexes, List<Vertex> _Vertices, List<EmitVertex> _Emitters, int mode ) {
            // ======= MODEL + EDGES ========
            if( _Indexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();

                EDGE_NumVerts = 0;
                EDGE_Vertices?.Dispose();
            }
            else {
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

            // ========= EMITTER VERTEX INSTANCES =========
            if(_Emitters.Count == 0 ) {
                EMIT_NumInstances = 0;
                EMIT_Instances?.Dispose();
            }
            else {
                Vector4[] data = new Vector4[_Emitters.Count * EMIT_INSTANCE_SPAN];
                for(int index = 0; index < _Emitters.Count; index++ ) {
                    var emit_ = _Emitters[index];
                    data[index] = new Vector4( emit_.Position[0], emit_.Position[1], emit_.Position[2], 0 );
                }

                EMIT_Instances?.Dispose();
                EMIT_Instances = Buffer.Create( _Device, BindFlags.VertexBuffer, data );
                EMIT_NumInstances = _Emitters.Count;
            }
        }
        public static int GetIdx( int faceIdx, int pointIdx, int span, int pointsPer ) {
            return span * ( faceIdx * pointsPer + pointIdx );
        }

        public override void OnDraw() {
            // ====== DRAW BASE =========
            if( NumVerts > 0 ) {
                _Ctx.PixelShader.Set( PShader );
                _Ctx.VertexShader.Set( VShader );
                _Ctx.InputAssembler.InputLayout = Layout;
                _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * MODEL_SPAN, 0 ) );
                _Ctx.Draw( NumVerts, 0 );
            }

            // ====== DRAW LINE =========
            if( ShowEdges ) {
                _Ctx.PixelShader.Set( EDGE_PShader );
                _Ctx.VertexShader.Set( EDGE_VShader );
                _Ctx.InputAssembler.InputLayout = EDGE_Layout;
                _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
                _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                _Ctx.GeometryShader.SetConstantBuffer( 0, RendersizeBuffer );
                if( EDGE_NumVerts > 0 ) {
                    _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EDGE_Vertices, Utilities.SizeOf<Vector4>() * EDGE_SPAN, 0 ) );
                    for( int i = 0; i < EDGE_NumVerts / 4; i++ ) {
                        _Ctx.Draw( 4, i * 4 );
                    }
                }
            }

            // ======= EMITTER ==========
            if (EMIT_NumInstances > 0 && ShowEmitter ) {
                _Ctx.PixelShader.Set( EMIT_PShader );
                _Ctx.VertexShader.Set( EMIT_VShader );
                _Ctx.InputAssembler.InputLayout = EMIT_Layout;
                _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );

                _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EMIT_Vertices, Utilities.SizeOf<Vector4>() * EMIT_SPAN, 0 ) );
                _Ctx.InputAssembler.SetVertexBuffers( 1, new VertexBufferBinding( EMIT_Instances, Utilities.SizeOf<Vector4>() * EMIT_INSTANCE_SPAN, 0 ) );

                _Ctx.DrawInstanced( EMIT_NumVerts, EMIT_NumInstances, 0, 0 );
            }
        }

        public override void OnDispose() {
            EMIT_Vertices?.Dispose();
            EMIT_Instances?.Dispose();
            EMIT_Layout?.Dispose();
            EMIT_Signature?.Dispose();
            EMIT_PShader?.Dispose();
            EMIT_VShader?.Dispose();
            EMIT_VertexShaderByteCode?.Dispose();
            EMIT_PixelShaderByteCode?.Dispose();

            EDGE_Vertices?.Dispose();
            EDGE_Layout?.Dispose();
            EDGE_Signature?.Dispose();
            EDGE_PShader?.Dispose();
            EDGE_VShader?.Dispose();
            EDGE_VertexShaderByteCode?.Dispose();
            EDGE_PixelShaderByteCode?.Dispose();

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
