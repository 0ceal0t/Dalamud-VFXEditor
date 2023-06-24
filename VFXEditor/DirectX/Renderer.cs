using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class Renderer {
        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        public Renderer( Device device, DeviceContext ctx ) {
            Device = device;
            Ctx = ctx;
        }

        protected void BeforeDraw( out RasterizerState oldState, out RenderTargetView[] oldRenderViews, out DepthStencilView oldDepthStencilView ) {
            oldState = Ctx.Rasterizer.State;
            oldRenderViews = Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out oldDepthStencilView );
        }

        protected void AfterDraw( RasterizerState oldState, RenderTargetView[] oldRenderViews, DepthStencilView oldDepthStencilView ) {
            Ctx.Rasterizer.State = oldState;
            Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
        }
    }
}
