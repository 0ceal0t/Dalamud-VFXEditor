using System;
using System.IO;
using Dalamud.Plugin;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using System.Reflection;
using System.Threading.Tasks;

namespace VFXEditor.Data.DirectX {
    public class DirectXManager {
        public static DirectXManager Manager;

        public Plugin Plugin;
        public Device Device;
        public DeviceContext Ctx;

        public string ShaderPath;
        public bool DoRender = true;

        public ModelPreview ModelView;
        public UVPreview UVView;
        public Gradient GradientView;

        // https://github.com/ackwell/BrowserHost/blob/32104cedd10715ca44710be1e57a36b6aa8c43c3/BrowserHost.Plugin/DxHandler.cs
        public DirectXManager(Plugin plugin) {
            Manager = this;

            Plugin = plugin;
            ShaderPath = Path.Combine( Plugin.TemplateLocation, "Shaders" );
            Device = plugin.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;
            ModelView = new ModelPreview( this );
            UVView = new UVPreview( this );
            GradientView = new Gradient( this );
        }

        public void BeforeDraw(out RasterizerState oldState, out RenderTargetView[] oldRenderViews, out DepthStencilView oldDepthStencilView ) {
            oldState = Ctx.Rasterizer.State;
            oldRenderViews = Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out oldDepthStencilView );
        }

        public void AfterDraw( RasterizerState oldState, RenderTargetView[] oldRenderViews, DepthStencilView oldDepthStencilView ) {
            Ctx.Rasterizer.State = oldState;
            Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
        }

        public void Save(Texture2D t, int width, int height, string path) {
            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
            var screenTexture = new Texture2D( Device, textureDesc );
            Ctx.CopyResource( t, screenTexture );

            var mapSource = Ctx.MapSubresource( screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None );

            var bitmap = new System.Drawing.Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
            var boundsRect = new System.Drawing.Rectangle( 0, 0, width, height );
            var mapDest = bitmap.LockBits( boundsRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat );
            var sourcePtr = mapSource.DataPointer;
            var destPtr = mapDest.Scan0;
            for( int y = 0; y < height; y++ ) {
                Utilities.CopyMemory( destPtr, sourcePtr, width * 4 );
                sourcePtr = IntPtr.Add( sourcePtr, mapSource.RowPitch );
                destPtr = IntPtr.Add( destPtr, mapDest.Stride );
            }
            bitmap.UnlockBits( mapDest );
            Ctx.UnmapSubresource( screenTexture, 0 );
            bitmap.Save( path );
        }

        public void Dispose() {
            DoRender = false;

            ModelView.Dispose();
            UVView.Dispose();
            GradientView.Dispose();

            Device = null;
            Ctx = null;
        }
    }
}
