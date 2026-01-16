using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Linq;
using VfxEditor;
using VfxEditor.DirectX.Drawable;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Model {
    public abstract class ModelDeferredRenderer<T> : ModelRenderer<T> where T : ModelDeferredInstance {
        protected readonly D3dDrawable Quad;
        protected readonly DepthStencilState QuadStencilState;

        protected ModelDeferredRenderer( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            // https://github.com/justinstenning/Direct3D-Rendering-Cookbook/blob/672312ae7545c388387a8fec92d8db41cc326804/Ch10_01DeferredRendering/ScreenAlignedQuadRenderer.cs#L20

            Quad = new( 1, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 )
                ] );

            Quad.SetVertexes( Device, [
                new( -1.0f, -1.0f, 1, 1 ),
                new( 1.0f, 1.0f, 1, 1 ),
                new( -1.0f, 1.0f, 1, 1 ),
                new( 1.0f, 1.0f, 1, 1 ),
                new( -1.0f, -1.0f, 1, 1 ),
                new( 1.0f, -1.0f, 1, 1 ),
            ], 6 );

            QuadStencilState = new( Device, new() {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Always
            } );
        }

        protected abstract void OnRenderUpdate( T instance );

        protected abstract void GBufferPass( T instance );

        protected abstract void QuadPass( T instance );

        protected override void RenderPasses( T instance ) {
            instance.Position.Clear( Ctx, new( 0 ) );
            instance.Normal.Clear( Ctx, new( 0 ) );
            instance.Color.Clear( Ctx, Plugin.Configuration.RendererBackground );
            instance.UV.Clear( Ctx, new( 0 ) );

            OnRenderUpdate( instance );

            // ======= G-BUFFERS PASS ======

            Ctx.OutputMerger.SetTargets( instance.StencilView, instance.DeferredTextures.Select( x => x.RenderTarget ).ToArray() );
            Ctx.PixelShader.SetSampler( 0, Sampler );

            GBufferPass( instance );
            Ctx.Flush();

            // ======= QUAD PASS ======

            Ctx.OutputMerger.SetTargets( instance.RenderTarget );
            Ctx.PixelShader.SetShaderResource( 0, instance.DepthResource );
            foreach( var (tex, idx) in instance.DeferredTextures.WithIndex() ) {
                Ctx.PixelShader.SetShaderResource( idx + 1, tex.Resource );
            }

            Ctx.OutputMerger.SetDepthStencilState( QuadStencilState );
            QuadPass( instance );
            Ctx.Flush();
        }

        public override void Dispose() {
            base.Dispose();
            Quad?.Dispose();
            QuadStencilState?.Dispose();
        }
    }
}
