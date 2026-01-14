using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public abstract class Renderer {
        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        // TODO: REMOVE THIS!
        private static int _Id = 0;
        public static int NewId => _Id++;
        public int CurrentRenderId { get; protected set; } = -1;

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

        public abstract void Dispose();

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
