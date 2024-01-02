using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public abstract class Renderer {
        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        public Renderer( Device device, DeviceContext ctx ) {
            Device = device;
            Ctx = ctx;
        }

        protected void BeforeDraw( out RasterizerState oldState, out RenderTargetView[] oldRenderViews, out DepthStencilView oldDepthStencilView, out DepthStencilState oldDepthStencilState ) {
            oldState = Ctx.Rasterizer.State;
            oldRenderViews = Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out oldDepthStencilView );
            oldDepthStencilState = Ctx.OutputMerger.GetDepthStencilState( out var _ );
        }

        protected void AfterDraw( RasterizerState oldState, RenderTargetView[] oldRenderViews, DepthStencilView oldDepthStencilView, DepthStencilState oldDepthStencilState ) {
            Ctx.Rasterizer.State = oldState;
            Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
            Ctx.OutputMerger.SetDepthStencilState( oldDepthStencilState );
        }

        public abstract void Draw();

        public abstract void Dispose();
    }
}
