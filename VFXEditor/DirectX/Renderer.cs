using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
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

        // Data is RGBA, mips are generated so that tiled textures don't alias
        public ShaderResourceView GetTexture( byte[] data, int height, int width, out Texture2D texture ) {
            texture = new( Device, new() {
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                SampleDescription = new SampleDescription( 1, 0 ),
            } );

            Ctx.UpdateSubresource( data, texture, 0, width * 4 );

            var view = new ShaderResourceView( Device, texture );
            Ctx.GenerateMips( view );
            return view;
        }

        public ShaderResourceView GetTextureArray( List<byte[]> layers, int height, int width, out Texture2D texture ) {
            texture = new( Device, new() {
                Width = width,
                Height = height,
                ArraySize = layers.Count,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                SampleDescription = new SampleDescription( 1, 0 ),
            } );

            var mipLevels = texture.Description.MipLevels;
            for( var i = 0; i < layers.Count; i++ ) {
                Ctx.UpdateSubresource( layers[i], texture, SharpDX.Direct3D11.Resource.CalculateSubResourceIndex( 0, i, mipLevels ), width * 4 );
            }

            var view = new ShaderResourceView( Device, texture );
            Ctx.GenerateMips( view );
            return view;
        }
    }
}