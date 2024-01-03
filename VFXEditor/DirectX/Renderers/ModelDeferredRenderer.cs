using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Renderers {
    public abstract class ModelDeferredRenderer : ModelRenderer {
        protected Texture2D ShadowDepthTexture;
        protected DepthStencilView GDepthView;
        protected ShaderResourceView ShadowDepthResource;

        protected ModelDeferredRenderer( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        protected override void ResizeResources() {
            base.ResizeResources();

            ShadowDepthTexture?.Dispose();
            ShadowDepthTexture = new Texture2D( Device, new() {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32_Typeless,
                Width = Width,
                Height = Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new( 1, 0 ),
                Usage = ResourceUsage.Default
            } );

            GDepthView?.Dispose();
            GDepthView = new DepthStencilView( Device, ShadowDepthTexture, new() {
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = Format.D32_Float,
            } );

            ShadowDepthResource?.Dispose();
            ShadowDepthResource = new ShaderResourceView( Device, ShadowDepthTexture, new() {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            } );
        }

        protected abstract void OnDraw();

        protected abstract void FinalPass();

        protected abstract void DepthPass();

        protected override void DrawPasses() {
            Ctx.ClearDepthStencilView( GDepthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0 );

            OnDraw();

            // ======= DEPTH PASS ======

            Ctx.OutputMerger.SetTargets( GDepthView );
            DepthPass();
            Ctx.Flush();

            // ======= FINAL PASS ======

            Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            FinalPass();
            Ctx.Flush();
        }

        public override void Dispose() {
            base.Dispose();
            ShadowDepthTexture?.Dispose();
            ShadowDepthResource?.Dispose();
        }
    }
}
