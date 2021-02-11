using System;
using System.IO;
using Dalamud.Plugin;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using System.Reflection;
using System.Threading.Tasks;

namespace VFXEditor {
    public class DirectXManager {
        public Plugin _plugin;
        public Device _Device;
        public DeviceContext _Ctx;
        public string ShaderPath;
        public bool DoRender = true;

        public Model3D Model;

        // https://github.com/ackwell/BrowserHost/blob/32104cedd10715ca44710be1e57a36b6aa8c43c3/BrowserHost.Plugin/DxHandler.cs

        public DirectXManager(Plugin plugin) {
            _plugin = plugin;
            ShaderPath = Path.Combine( plugin.TemplateLocation, "Shaders" );

            //=== USE REFLECTION FOR NOW =========
            _Device = plugin.PluginInterface.UiBuilder.Device;
            _Ctx = _Device.ImmediateContext;

            Model = new Model3D( this );

            PluginLog.Log( "Set up DirectX" );
        }

        public void Draw() {
            if( !_plugin.MainUI.Visible ) // no need to render
                return;
            var oldState = _Ctx.Rasterizer.State;
            var oldRenderViews = _Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out var oldDepthStencilView );

            Model.Draw();

            _Ctx.Rasterizer.State = oldState;
            _Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
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
            var screenTexture = new Texture2D( _Device, textureDesc );
            _Ctx.CopyResource( t, screenTexture );

            var mapSource = _Ctx.MapSubresource( screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None );

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
            _Ctx.UnmapSubresource( screenTexture, 0 );
            bitmap.Save( path );
        }

        public void Dispose() {
            DoRender = false;

            Model.Dispose();

            _Device = null;
            _Ctx = null;
        }
    }
}
