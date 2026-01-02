using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.DirectX.Drawable;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Renderers {
    public abstract class ModelDeferredRenderer : ModelRenderer {
        protected readonly D3dDrawable Quad;
        protected readonly DepthStencilState QuadStencilState;

        protected ShaderResourceView DepthResource;

        protected readonly DeferredTexture Position = new();
        protected readonly DeferredTexture Normal = new();
        protected readonly DeferredTexture Color = new();
        protected readonly DeferredTexture UV = new();

        protected List<DeferredTexture> DeferredTextures = [];

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

        protected override void ResizeResources() {
            base.ResizeResources();

            if( DeferredTextures.Count == 0 ) {
                DeferredTextures.AddRange( [
                    Position,
                    Normal,
                    Color,
                    UV
                ] );
            }

            DeferredTextures.ForEach( x => x.Resize( Device, Width, Height ) );

            DepthResource?.Dispose();
            DepthResource = new ShaderResourceView( Device, StencilTexture, new() {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            } );
        }

        protected abstract void OnDrawUpdate();

        protected abstract void GBufferPass();

        protected abstract void QuadPass();

        protected override void DrawPasses() {
            Position.Clear( Ctx, new( 0 ) );
            Normal.Clear( Ctx, new( 0 ) );
            Color.Clear( Ctx, Plugin.Configuration.RendererBackground );
            UV.Clear( Ctx, new( 0 ) );

            OnDrawUpdate();

            // ======= G-BUFFERS PASS ======

            Ctx.OutputMerger.SetTargets( StencilView, DeferredTextures.Select( x => x.RenderTarget ).ToArray() );
            Ctx.PixelShader.SetSampler( 0, Sampler );

            GBufferPass();
            Ctx.Flush();

            // ======= QUAD PASS ======
            Ctx.OutputMerger.SetTargets( RenderTarget );
            Ctx.PixelShader.SetShaderResource( 0, DepthResource );
            foreach( var (tex, idx) in DeferredTextures.WithIndex() ) {
                Ctx.PixelShader.SetShaderResource( idx + 1, tex.Resource );
            }

            Ctx.OutputMerger.SetDepthStencilState( QuadStencilState );
            QuadPass();
            Ctx.Flush();
        }

        public override void Dispose() {
            base.Dispose();
            DeferredTextures.ForEach( x => x.Dispose() );
            DepthResource?.Dispose();
            Quad?.Dispose();
            QuadStencilState?.Dispose();
        }
    }
}
