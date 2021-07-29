using System;
using System.IO;
using AVFXLib.Models;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VFXEditor.Data.DirectX {
    public class GenericDirectX {
        protected Device Device;
        protected DeviceContext Ctx;

        public GenericDirectX(Device device, DeviceContext ctx) {
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
