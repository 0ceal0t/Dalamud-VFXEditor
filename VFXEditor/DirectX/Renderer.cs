using Dalamud.Bindings.ImGui;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public abstract class Renderer {
        public bool NeedsUpdate { get; set; } = false;

        protected RenderInstance LoadedInstance;
        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        public Renderer( Device device, DeviceContext ctx ) {
            Device = device;
            Ctx = ctx;
        }

        protected void OnUpdate( int renderId, RenderInstance instance ) {
            instance.CurrentRenderId = renderId;
            instance.NeedsRender = true;
            NeedsUpdate = false;
            LoadedInstance = instance;
        }

        protected void BeforeRender( out RasterizerState oldState, out RenderTargetView[] oldRenderViews, out DepthStencilView oldDepthStencilView, out DepthStencilState oldDepthStencilState ) {
            oldState = Ctx.Rasterizer.State;
            oldRenderViews = Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out oldDepthStencilView );
            oldDepthStencilState = Ctx.OutputMerger.GetDepthStencilState( out var _ );
        }

        protected void AfterRender( RasterizerState oldState, RenderTargetView[] oldRenderViews, DepthStencilView oldDepthStencilView, DepthStencilState oldDepthStencilState ) {
            Ctx.Rasterizer.State = oldState;
            Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
            Ctx.OutputMerger.SetDepthStencilState( oldDepthStencilState );
        }

        public virtual void Dispose() {
            LoadedInstance = null;
        }

        public ShaderResourceView GetTexture( byte[] data, int height, int width, out Texture2D texture ) {
            var stream = DataStream.Create( data, true, true );
            var rect = new DataRectangle( stream.DataPointer, width * 4 );
            texture = new( Device, new() {
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription( 1, 0 ),
            }, rect );

            return new ShaderResourceView( Device, texture );
        }
    }
}
