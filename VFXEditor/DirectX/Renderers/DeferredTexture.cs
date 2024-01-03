using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Renderers {
    public class DeferredTexture {
        public Texture2D Texture { get; private set; }
        public ShaderResourceView Resource { get; private set; }
        public RenderTargetView RenderTarget { get; private set; }

        public void Resize( Device device, int width, int height ) {
            Texture?.Dispose();
            Resource?.Dispose();
            RenderTarget?.Dispose();

            Texture = new Texture2D( device, new() {
                Format = Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new( 1, 0 ),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            } );

            Resource = new ShaderResourceView( device, Texture );

            RenderTarget = new RenderTargetView( device, Texture );
        }

        public void Clear( DeviceContext ctx, System.Numerics.Vector4 background ) {
            ctx.ClearRenderTargetView( RenderTarget, new Color4(
                background.X,
                background.Y,
                background.Z,
                background.W
            ) );
        }

        public void Dispose() {
            Texture?.Dispose();
            Resource?.Dispose();
            RenderTarget?.Dispose();
        }
    }
}
