using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Drawable {
    public class D3dDrawable {
        public readonly int Span;
        public readonly PrimitiveTopology Toplogy;

        public bool ShaderError { get; protected set; } = false;

        public readonly bool UseInstances;

        public int Count { get; protected set; } = 0;
        public int InstanceCount { get; protected set; } = 0;

        public Buffer Instances { get; protected set; }
        public Buffer Data { get; protected set; }

        private readonly Dictionary<PassType, D3dPass> Passes = [];
        private readonly InputElement[] Layout;

        public bool DoDraw => !ShaderError && Count > 0 && ( !UseInstances || InstanceCount > 0 );

        public D3dDrawable( int span, bool instances, InputElement[] layout, PrimitiveTopology topology = PrimitiveTopology.TriangleList ) {
            UseInstances = instances;
            Span = span;
            Layout = layout;
            Toplogy = topology;
        }

        public void AddPass( Device device, PassType type, string path, ShaderPassFlags flags ) {
            try {
                Passes[type] = new( device, path, flags, Layout, Toplogy );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error compiling shaders from {path}" );
                ShaderError = true;
            }
        }

        public D3dPass? GetPass( PassType type ) => Passes.TryGetValue( type, out var pass ) ? pass : null;

        // ====== SET DATA =============

        public void ClearVertexes() {
            Count = 0;
            Data?.Dispose();
        }

        public void SetVertexes( Buffer buffer, int count ) {
            Count = count;
            Data = buffer;
        }

        public void SetVertexes( Device device, Vector4[] data, int count ) {
            Count = count;
            Data?.Dispose();
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        public void ClearInstances() {
            InstanceCount = 0;
            Instances?.Dispose();
        }

        public void SetInstances( Device device, Matrix[] data, int count ) {
            InstanceCount = count;
            Instances?.Dispose();
            Instances = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        // ==============================

        public void SetupPass( DeviceContext ctx, PassType type ) {
            if( !DoDraw ) return;
            var pass = GetPass( type );
            if( pass == null ) return;
            pass.Setup( ctx );
        }
        public void SetConstantBuffers( DeviceContext ctx, Buffer vertex, Buffer pixel ) => SetConstantBuffers( ctx, [vertex], [pixel] );

        public virtual void SetConstantBuffers( DeviceContext ctx, List<Buffer> vertex, List<Buffer> pixel ) {
            if( !DoDraw ) return;
            foreach( var (buffer, idx) in vertex.WithIndex() ) {
                ctx.VertexShader.SetConstantBuffer( idx, buffer );
            }

            foreach( var (buffer, idx) in pixel.WithIndex() ) {
                ctx.PixelShader.SetConstantBuffer( idx, buffer );
            }
        }

        public void Draw( DeviceContext ctx, PassType type ) => Draw( ctx, type, [], [] );

        public void Draw( DeviceContext ctx, PassType type, Buffer vertex, Buffer pixel ) => Draw( ctx, type, [vertex], [pixel] );

        public void Draw( DeviceContext ctx, PassType type, List<Buffer> vertex, List<Buffer> pixel ) {
            if( !DoDraw ) return;
            SetupPass( ctx, type );
            SetConstantBuffers( ctx, vertex, pixel );
            DrawVertexes( ctx );
        }

        public void DrawVertexes( DeviceContext ctx ) {
            if( !DoDraw ) return;

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

        public virtual void Dispose() {
            Data?.Dispose();
            Instances?.Dispose();
            foreach( var pass in Passes ) pass.Value?.Dispose();
            Passes.Clear();
        }
    }
}
