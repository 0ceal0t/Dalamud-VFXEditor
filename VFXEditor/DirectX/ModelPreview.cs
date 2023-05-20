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
using VfxEditor.AvfxFormat;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class ModelPreview : ModelRenderer {
        public enum RenderMode : int {
            Color = 1,
            Uv1 = 2,
            Uv2 = 3
        }

        public bool ShowEdges = true;
        public bool ShowEmitter = true;
        public static readonly Vector4 LineColor = new( 1, 0, 0, 1 );

        // ======= BASE MODEL =======
        private static readonly int ModelSpan = 3; // position, color, normal
        private int NumVerts;
        private Buffer Vertices;
        private readonly CompilationResult VertexShaderByteCode;
        private readonly CompilationResult PixelShaderByteCode;
        private readonly PixelShader PShader;
        private readonly VertexShader VShader;
        private readonly ShaderSignature Signature;
        private readonly InputLayout Layout;

        // ======= EDGES ==========
        private static readonly int EdgeSpan = 2;
        private int EdgeNumVerts;
        private Buffer EdgeVertices;
        private readonly CompilationResult EdgeVertexShaderByteCode;
        private readonly CompilationResult EdgePixelShaderByteCode;
        private readonly PixelShader EdgePShader;
        private readonly VertexShader EdgeVShader;
        private readonly ShaderSignature EdgeSignature;
        private readonly InputLayout EdgeLayout;

        // ======= EMITTERS ==========
        private static readonly int EmitSpan = 2; // position, normal
        private readonly int EmitNumVerts;
        private int EmitNumInstances;
        private readonly Buffer EmitVertices;
        private Buffer EmitInstances;
        private readonly CompilationResult EmitVertexShaderByteCode;
        private readonly CompilationResult EmitPixelShaderByteCode;
        private readonly PixelShader EmitPShader;
        private readonly VertexShader EmitVShader;
        private readonly ShaderSignature EmitSignature;
        private readonly InputLayout EmitLayout;

        public readonly bool ShaderError = false;

        public ModelPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            // ======= BASE MODEL =========
            NumVerts = 0;
            Vertices = null;

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

            // ======= MODEL EDGES ========
            EdgeNumVerts = 0;
            EdgeVertices = null;

            try {
                EdgeVertexShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "ModelEdge_VS.fx" ), "VS", "vs_4_0" );
                EdgePixelShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "ModelEdge_PS.fx" ), "PS", "ps_4_0" );
                EdgeVShader = new VertexShader( Device, EdgeVertexShaderByteCode );
                EdgePShader = new PixelShader( Device, EdgePixelShaderByteCode );
                EdgeSignature = ShaderSignature.GetInputSignature( EdgeVertexShaderByteCode );
                EdgeLayout = new InputLayout( Device, EdgeSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                } );
            }
            catch( Exception e ) {
                PluginLog.Error( "Error compiling shaders", e );
                ShaderError = true;
            }


            // ======= EMITTER VERTICES ========

            var builder = new MeshBuilder( true, false );
            builder.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitX, Vector3.UnitY, 0.25f, 0.5f, true );
            var emit = builder.ToMesh();
            emit.Normals = emit.CalculateNormals();

            var normals = emit.Normals;
            var positions = emit.Positions;
            var indexes = emit.Indices;

            var data = new Vector4[indexes.Count * EmitSpan];

            for( var index = 0; index < indexes.Count; index++ ) {
                var pointIdx = indexes[index];

                var position = positions[pointIdx];
                var normal = normals[pointIdx];

                var dataIdx = index * EmitSpan;
                data[dataIdx] = new Vector4( position.X, position.Y, position.Z, 1 ); // POSITION
                data[dataIdx + 1] = new Vector4( normal.X, normal.Y, normal.Z, 0 ); // NORMAL
            }

            EmitNumVerts = indexes.Count;
            EmitVertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );

            EmitNumInstances = 0;
            EmitInstances = null;

            try {
                EmitVertexShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "EmitterVertex_VS.fx" ), "VS", "vs_4_0" );
                EmitPixelShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "EmitterVertex_PS.fx" ), "PS", "ps_4_0" );
                EmitVShader = new VertexShader( Device, EmitVertexShaderByteCode );
                EmitPShader = new PixelShader( Device, EmitPixelShaderByteCode );
                EmitSignature = ShaderSignature.GetInputSignature( EmitVertexShaderByteCode );
                EmitLayout = new InputLayout( Device, EmitSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0, InputClassification.PerVertexData, 0),
                    new InputElement("INSTANCE", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1)
                } );
            }
            catch( Exception e ) {
                PluginLog.Error( "Error compiling shaders", e );
                ShaderError = true;
            }
        }

        public void LoadModel( AvfxModel model, RenderMode mode ) => LoadModel( model.Indexes.Indexes, model.Vertexes.Vertexes, model.CombinedEmitVertexes, mode );

        public void LoadModel( List<AvfxIndex> modelIndexes, List<AvfxVertex> modelVertexes, List<UiEmitVertex> modelEmitters, RenderMode mode ) {
            if( modelIndexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();

                EdgeNumVerts = 0;
                EdgeVertices?.Dispose();
            }
            else {
                var data = new Vector4[modelIndexes.Count * 3 * ModelSpan]; // 3 vertices per face
                var edgeData = new Vector4[modelIndexes.Count * 4 * EdgeSpan]; // 4 points per loop

                for( var index = 0; index < modelIndexes.Count; index++ ) { // each face
                    var indexes = new int[] { modelIndexes[index].I1, modelIndexes[index].I2, modelIndexes[index].I3 };
                    for( var j = 0; j < indexes.Length; j++ ) { // push all 3 vertices per face
                        var idx = GetIdx( index, j, ModelSpan, 3 );
                        var v = modelVertexes[indexes[j]];

                        data[idx + 0] = new Vector4( v.Position[0], v.Position[1], v.Position[2], 1.0f );
                        data[idx + 1] = mode switch {
                            RenderMode.Color => new Vector4( v.Color[0] / 255, v.Color[1] / 255, v.Color[2] / 255, 1.0f ),
                            RenderMode.Uv1 => new Vector4( v.UV1[0] + 0.5f, 0, v.UV1[1] + 0.5f, 1.0f ),
                            RenderMode.Uv2 => new Vector4( v.UV2[2] + 0.5f, 0, v.UV2[3] + 0.5f, 1.0f ),
                            _ => throw new System.NotImplementedException()
                        };
                        data[idx + 2] = new Vector4( v.Normal[0], v.Normal[1], v.Normal[2], 0 );

                        // ========= COPY OVER EDGE DATA ==========
                        var edgeIdx = GetIdx( index, j, EdgeSpan, 4 );
                        edgeData[edgeIdx + 0] = data[idx + 0];
                        edgeData[edgeIdx + 1] = LineColor;
                        if( j == 0 ) { // loop back around
                            var lastEdgeIdx = GetIdx( index, 3, EdgeSpan, 4 );
                            edgeData[lastEdgeIdx + 0] = data[idx + 0];
                            edgeData[lastEdgeIdx + 1] = LineColor;
                        }
                    }
                }

                Vertices?.Dispose();
                Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                NumVerts = modelIndexes.Count * 3;

                EdgeVertices?.Dispose();
                EdgeVertices = Buffer.Create( Device, BindFlags.VertexBuffer, edgeData );
                EdgeNumVerts = modelIndexes.Count * 4;
            }

            // ========= EMITTER VERTEX INSTANCES =========
            if( modelEmitters.Count == 0 ) {
                EmitNumInstances = 0;
                EmitInstances?.Dispose();
            }
            else {
                var data = new Matrix[modelEmitters.Count];
                for( var idx = 0; idx < modelEmitters.Count; idx++ ) {
                    var emitter = modelEmitters[idx];

                    data[idx] = Matrix.Translation( new Vector3( emitter.Position.X, emitter.Position.Y, emitter.Position.Z ) );
                }

                EmitInstances?.Dispose();
                EmitInstances = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                EmitNumInstances = modelEmitters.Count;
            }

            UpdateDraw();
        }

        public override void OnDraw() {
            if( ShaderError ) return;

            // ====== DRAW BASE =========
            if( NumVerts > 0 ) {
                Ctx.PixelShader.Set( PShader );
                Ctx.VertexShader.Set( VShader );
                Ctx.InputAssembler.InputLayout = Layout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                Ctx.Draw( NumVerts, 0 );
            }

            // ====== DRAW LINE =========
            if( ShowEdges ) {
                Ctx.PixelShader.Set( EdgePShader );
                Ctx.VertexShader.Set( EdgeVShader );
                Ctx.InputAssembler.InputLayout = EdgeLayout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
                Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                Ctx.GeometryShader.SetConstantBuffer( 0, RendersizeBuffer );
                if( EdgeNumVerts > 0 ) {
                    Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EdgeVertices, Utilities.SizeOf<Vector4>() * EdgeSpan, 0 ) );
                    for( var i = 0; i < EdgeNumVerts / 4; i++ ) {
                        Ctx.Draw( 4, i * 4 );
                    }
                }
            }

            // ======= EMITTER ==========
            if( EmitNumInstances > 0 && ShowEmitter ) {
                Ctx.PixelShader.Set( EmitPShader );
                Ctx.VertexShader.Set( EmitVShader );
                Ctx.InputAssembler.InputLayout = EmitLayout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EmitVertices, Utilities.SizeOf<Vector4>() * EmitSpan, 0 ) );
                Ctx.InputAssembler.SetVertexBuffers( 1, new VertexBufferBinding( EmitInstances, Utilities.SizeOf<Matrix>(), 0 ) );

                Ctx.DrawInstanced( EmitNumVerts, EmitNumInstances, 0, 0 );
            }
        }

        public override void OnDispose() {
            EmitVertices?.Dispose();
            EmitInstances?.Dispose();
            EmitLayout?.Dispose();
            EmitSignature?.Dispose();
            EmitPShader?.Dispose();
            EmitVShader?.Dispose();
            EmitVertexShaderByteCode?.Dispose();
            EmitPixelShaderByteCode?.Dispose();

            EdgeVertices?.Dispose();
            EdgeLayout?.Dispose();
            EdgeSignature?.Dispose();
            EdgePShader?.Dispose();
            EdgeVShader?.Dispose();
            EdgeVertexShaderByteCode?.Dispose();
            EdgePixelShaderByteCode?.Dispose();

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
