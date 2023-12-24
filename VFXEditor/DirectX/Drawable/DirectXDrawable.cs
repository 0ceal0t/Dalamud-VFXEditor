using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Drawable {
    public class DirectXDrawable {
        public readonly int Span;

        public int Count { get; protected set; } = 0;
        public bool ShaderError { get; protected set; } = false;

        public readonly bool UseGeometryShader;
        public readonly CompilationResult CompiledGeometryShader;
        public readonly GeometryShader GeometryShader;

        public readonly bool UseInstances;
        public int InstanceCount { get; protected set; } = 0;
        public Buffer Instances { get; protected set; }

        public Buffer Data { get; protected set; }
        public readonly CompilationResult CompiledVertexShader;
        public readonly CompilationResult CompiledPixelShader;
        public readonly PixelShader PixelShader;
        public readonly VertexShader VertexShader;
        public readonly ShaderSignature Signature;
        public readonly InputLayout Layout;

        public DirectXDrawable( Device device, string shader, int span, bool geometryShader, bool instances, InputElement[] layout ) {
            UseGeometryShader = geometryShader;
            UseInstances = instances;
            Span = span;

            try {
                CompiledVertexShader = ShaderBytecode.CompileFromFile( shader, "VS", "vs_4_0", include: DirectXManager.IncludeHandler );
                CompiledPixelShader = ShaderBytecode.CompileFromFile( shader, "PS", "ps_4_0", include: DirectXManager.IncludeHandler );
                VertexShader = new VertexShader( device, CompiledVertexShader );
                PixelShader = new PixelShader( device, CompiledPixelShader );
                Signature = ShaderSignature.GetInputSignature( CompiledVertexShader );
                Layout = new InputLayout( device, Signature, layout );

                if( UseGeometryShader ) {
                    CompiledGeometryShader = ShaderBytecode.CompileFromFile( shader, "GS", "gs_4_0", include: DirectXManager.IncludeHandler );
                    GeometryShader = new GeometryShader( device, CompiledGeometryShader );
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error compiling shaders" );
                ShaderError = true;
            }
        }

        public virtual void ClearVertexes() {
            Count = 0;
            Data?.Dispose();
        }

        public virtual void SetVertexes( Device device, Vector4[] data, int count ) {
            Count = count;
            Data?.Dispose();
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        public virtual void ClearInstances() {
            InstanceCount = 0;
            Instances?.Dispose();
        }

        public virtual void SetInstances( Device device, Matrix[] data, int count ) {
            InstanceCount = count;
            Instances?.Dispose();
            Instances = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        public virtual void Draw( DeviceContext ctx, Buffer vertexBuffer, Buffer pixelBuffer ) => Draw( ctx, new List<Buffer>() { vertexBuffer }, new List<Buffer>() { pixelBuffer } );

        public virtual void Draw( DeviceContext ctx, List<Buffer> vertexBuffers, List<Buffer> pixelBuffers ) {
            if( ShaderError ) return;
            if( Count == 0 || ( UseInstances && InstanceCount == 0 ) ) return;

            SetupCtx( ctx, vertexBuffers, pixelBuffers );
            ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Data, Utilities.SizeOf<Vector4>() * Span, 0 ) );

            if( UseInstances ) {
                ctx.InputAssembler.SetVertexBuffers( 1, new VertexBufferBinding( Instances, Utilities.SizeOf<Matrix>(), 0 ) );
                ctx.DrawInstanced( Count, InstanceCount, 0, 0 );
            }
            else {
                ctx.Draw( Count, 0 );
            }

            ctx.GeometryShader.Set( null );
        }

        public void SetupCtx( DeviceContext ctx, List<Buffer> vertexBuffers, List<Buffer> pixelBuffers ) {
            ctx.PixelShader.Set( PixelShader );
            ctx.GeometryShader.Set( GeometryShader );
            ctx.VertexShader.Set( VertexShader );
            ctx.InputAssembler.InputLayout = Layout;
            ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            foreach( var (buffer, idx) in vertexBuffers.WithIndex() ) {
                ctx.VertexShader.SetConstantBuffer( idx, buffer );
            }

            foreach( var (buffer, idx) in pixelBuffers.WithIndex() ) {
                ctx.PixelShader.SetConstantBuffer( idx, buffer );
            }
        }

        public virtual void Dispose() {
            Data?.Dispose();
            Instances?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PixelShader?.Dispose();
            VertexShader?.Dispose();
            GeometryShader?.Dispose();
            CompiledPixelShader?.Dispose();
            CompiledVertexShader?.Dispose();
            CompiledGeometryShader?.Dispose();
        }
    }
}
