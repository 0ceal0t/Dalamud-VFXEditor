using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace VFXEditor.DirectX.Instance {
    public class GradientInstance : RenderInstance {
        public nint Output => ShaderView.NativePointer;

        public Texture2D DepthTexture { get; protected set; }
        public DepthStencilView DepthView { get; protected set; }
        public Texture2D RenderTexture { get; protected set; }
        public ShaderResourceView ShaderView { get; protected set; }
        public RenderTargetView RenderView { get; protected set; }

        public readonly int Width = 900;
        public readonly int Height = 100;

        public GradientInstance() {
            ResizeResources();
        }

        protected override void ResizeResources() {
            RenderTexture?.Dispose();
            RenderTexture = new Texture2D( Device, new Texture2DDescription() {
                Format = Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription( 1, 0 ),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            } );
            ShaderView?.Dispose();
            ShaderView = new ShaderResourceView( Device, RenderTexture );
            RenderView?.Dispose();
            RenderView = new RenderTargetView( Device, RenderTexture );

            DepthTexture?.Dispose();
            DepthTexture = new Texture2D( Device, new Texture2DDescription() {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription( 1, 0 ),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            } );
            DepthView?.Dispose();
            DepthView = new DepthStencilView( Device, DepthTexture );
        }

        public override void Dispose() {
            DepthTexture?.Dispose();
            DepthView?.Dispose();
            RenderTexture?.Dispose();
            ShaderView?.Dispose();
            RenderView?.Dispose();
        }
    }
}
