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
        public enum RenderMode {
            Color,
            Uv1,
            Uv2,
            Normal
        }

        // ======= BASE MODEL =======
        private static readonly int ModelSpan = 3; // position, color, normal
        private int NumVerts;
        private Buffer Vertices;
        private readonly CompilationResult CompiledVS;
        private readonly CompilationResult CompiledPS;
        private readonly CompilationResult CompiledGS;
        private readonly PixelShader PS;
        private readonly VertexShader VS;
        private readonly GeometryShader GS;
        private readonly ShaderSignature Signature;
        private readonly InputLayout Layout;

        // ======= EMITTERS ==========
        private static readonly int EmitSpan = 2; // position, normal
        private readonly int EmitNumVerts;
        private int EmitNumInstances;
        private readonly Buffer EmitVertices;
        private Buffer EmitInstances;
        private readonly CompilationResult EmitCompiledVS;
        private readonly CompilationResult EmitCompiledPS;
        private readonly PixelShader EmitPS;
        private readonly VertexShader EmitVS;
        private readonly ShaderSignature EmitSignature;
        private readonly InputLayout EmitLayout;

        public readonly bool ShaderError = false;

        public ModelPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            // ======= BASE MODEL =========

            NumVerts = 0;
            Vertices = null;

            try {
                CompiledVS = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "ModelPreview.fx" ), "VS", "vs_4_0" );
                CompiledPS = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "ModelPreview.fx" ), "PS", "ps_4_0" );
                CompiledGS = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "ModelPreview.fx" ), "GS", "gs_4_0" );
                VS = new VertexShader( Device, CompiledVS );
                PS = new PixelShader( Device, CompiledPS );
                GS = new GeometryShader( Device, CompiledGS );
                Signature = ShaderSignature.GetInputSignature( CompiledVS );
                Layout = new InputLayout( Device, Signature, new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "COLOR", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                } );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error compiling shaders" );
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
                EmitCompiledVS = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "Emitter.fx" ), "VS", "vs_4_0" );
                EmitCompiledPS = ShaderBytecode.CompileFromFile( Path.Combine( shaderPath, "Emitter.fx" ), "PS", "ps_4_0" );
                EmitVS = new VertexShader( Device, EmitCompiledVS );
                EmitPS = new PixelShader( Device, EmitCompiledPS );
                EmitSignature = ShaderSignature.GetInputSignature( EmitCompiledVS );
                EmitLayout = new InputLayout( Device, EmitSignature, new InputElement[] {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0, InputClassification.PerVertexData, 0),
                    new InputElement("INSTANCE", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1)
                } );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error compiling shaders" );
                ShaderError = true;
            }
        }

        public void LoadModel( AvfxModel model, RenderMode mode ) => LoadModel( model.Indexes.Indexes, model.Vertexes.Vertexes, model.CombinedEmitVertexes, mode );

        public void LoadModel( List<AvfxIndex> modelIndexes, List<AvfxVertex> modelVertexes, List<UiEmitVertex> modelEmitters, RenderMode mode ) {
            if( modelIndexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();
            }
            else {
                var data = new Vector4[modelIndexes.Count * 3 * ModelSpan]; // 3 vertices per face

                for( var index = 0; index < modelIndexes.Count; index++ ) { // each face
                    var indexes = new int[] { modelIndexes[index].I1, modelIndexes[index].I2, modelIndexes[index].I3 };

                    for( var j = 0; j < indexes.Length; j++ ) { // push all 3 vertices per face
                        var idx = GetIdx( index, j, ModelSpan, 3 );
                        var vertex = modelVertexes[indexes[j]];

                        var normal = new Vector3( vertex.Normal[0], vertex.Normal[1], vertex.Normal[2] );
                        var color = new Vector3( vertex.Color[0], vertex.Color[1], vertex.Color[2] );
                        var uv1 = new Vector4( vertex.Uv1[0] + 0.5f, 0, vertex.Uv1[1] + 0.5f, 1.0f );
                        var uv2 = new Vector4( vertex.Uv2[2] + 0.5f, 0, vertex.Uv2[3] + 0.5f, 1.0f );

                        data[idx + 0] = new Vector4( vertex.Position[0], vertex.Position[1], vertex.Position[2], 1.0f );
                        data[idx + 1] = mode switch {
                            RenderMode.Color => new Vector4( color / 255, 1.0f ),
                            RenderMode.Uv1 => uv1,
                            RenderMode.Uv2 => uv2,
                            RenderMode.Normal => new Vector4( normal.Normalized(), 1.0f ),
                            _ => throw new NotImplementedException()
                        };
                        data[idx + 2] = new Vector4( normal, 0 );
                    }
                }

                Vertices?.Dispose();
                Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                NumVerts = modelIndexes.Count * 3;
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

                    var pos = new Vector3( emitter.Position.X, emitter.Position.Y, emitter.Position.Z );
                    var rot = GetEmitterRotationQuat( new Vector3( emitter.Normal.X, emitter.Normal.Y, emitter.Normal.Z ) );

                    data[idx] = Matrix.AffineTransformation( 1f, rot, pos );
                }

                EmitInstances?.Dispose();
                EmitInstances = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                EmitNumInstances = modelEmitters.Count;
            }

            UpdateDraw();
        }

        private static Quaternion GetEmitterRotationQuat( Vector3 normal ) {
            var originalNormal = Vector3.UnitY;
            if( normal.Equals( originalNormal ) ) return Quaternion.Identity;

            var rotationAxis = Vector3.Cross( normal, originalNormal );
            if( rotationAxis.Length() == 0f ) { // N = -N'
                return Quaternion.RotationAxis( Vector3.UnitX, ( float )Math.PI );
            }

            var rotationAngle = Math.Acos( Vector3.Dot( normal, originalNormal ) / ( normal.Length() * originalNormal.Length() ) );

            return Quaternion.RotationAxis( rotationAxis, ( float )rotationAngle );
        }

        public override void OnDraw() {
            if( ShaderError ) return;

            // ====== DRAW BASE =========

            if( NumVerts > 0 ) {
                Ctx.PixelShader.Set( PS );
                Ctx.GeometryShader.Set( GS );
                Ctx.VertexShader.Set( VS );
                Ctx.InputAssembler.InputLayout = Layout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Ctx.VertexShader.SetConstantBuffer( 0, VSBuffer );
                Ctx.PixelShader.SetConstantBuffer( 0, PSBuffer );

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );

                Ctx.Draw( NumVerts, 0 );
            }

            // ======= EMITTER ==========

            if( EmitNumInstances > 0 && Plugin.Configuration.ModelShowEmitters ) {
                Ctx.PixelShader.Set( EmitPS );
                Ctx.VertexShader.Set( EmitVS );
                Ctx.GeometryShader.Set( null );
                Ctx.InputAssembler.InputLayout = EmitLayout;
                Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Ctx.VertexShader.SetConstantBuffer( 0, VSBuffer );
                Ctx.PixelShader.SetConstantBuffer( 0, PSBuffer );

                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EmitVertices, Utilities.SizeOf<Vector4>() * EmitSpan, 0 ) );
                Ctx.InputAssembler.SetVertexBuffers( 1, new VertexBufferBinding( EmitInstances, Utilities.SizeOf<Matrix>(), 0 ) );

                Ctx.DrawInstanced( EmitNumVerts, EmitNumInstances, 0, 0 );
            }

            Ctx.GeometryShader.Set( null );
        }

        public override void OnDispose() {
            EmitVertices?.Dispose();
            EmitInstances?.Dispose();
            EmitLayout?.Dispose();
            EmitSignature?.Dispose();
            EmitPS?.Dispose();
            EmitVS?.Dispose();
            EmitCompiledVS?.Dispose();
            EmitCompiledPS?.Dispose();

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
