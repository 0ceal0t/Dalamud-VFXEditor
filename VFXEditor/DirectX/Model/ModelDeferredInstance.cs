using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;

namespace VfxEditor.DirectX.Model {
    public class ModelDeferredInstance : ModelInstance {
        public ShaderResourceView DepthResource { get; protected set; }

        public readonly DeferredTexture Position = new();
        public readonly DeferredTexture Normal = new();
        public readonly DeferredTexture Color = new();
        public readonly DeferredTexture UV = new();

        public readonly List<DeferredTexture> DeferredTextures = [];

        public ModelDeferredInstance() : base() {
            ResizeResources();
        }

        protected override void ResizeResources() {
            base.ResizeResources();

            if( DeferredTextures.Count == 0 ) { // Init
                DeferredTextures.AddRange( [
                    Position,
                    Normal,
                    Color,
                    UV
                ] );
            }

            DeferredTextures.ForEach( x => x.Resize( Device, Width, Height ) );

            DepthResource?.Dispose();
            DepthResource = new ShaderResourceView( Device, StencilTexture, new() {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            } );
        }

        public override void Dispose() {
            base.Dispose();
            DeferredTextures.ForEach( x => x.Dispose() );
            DepthResource?.Dispose();
        }
    }
}
