using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.DirectX.Drawable;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Renderers {
    public class GradientRenderer : Renderer {
        public nint Output => ShaderView.NativePointer;

        private readonly int Width = 900;
        private readonly int Height = 100;

        private RasterizerState State;
        private Texture2D DepthTexture;
        private DepthStencilView DepthView;
        private Texture2D RenderTexture;
        private ShaderResourceView ShaderView;
        private RenderTargetView RenderView;

        private readonly D3dDrawable Gradient;

        public GradientRenderer( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            RefreshRasterizeState();
            ResizeResources();

            Gradient = new( 2, false,
                [
                    new("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                ] );
            Gradient.AddPass( device, PassType.Final, Path.Combine( shaderPath, "gradient.fx" ), ShaderPassFlags.Pixel );
        }

        public void SetGradient( int renderId, List<List<(int, System.Numerics.Vector3)>> rows ) {
            CurrentRenderId = renderId;

            if( rows.Count == 0 || rows.Min( x => x.Count ) < 2 ) {
                Gradient.ClearVertexes();
                Draw();
                return;
            }

            // =======================

            var data = new List<Vector4>();
            var count = 0;

            var startTime = ( float )rows[0][0].Item1;
            var endTime = ( float )rows[0][^1].Item1;
            var timeDiff = endTime - startTime;

            var rowPortion = 1f / rows.Count;
            foreach( var (row, rowIdx) in rows.WithIndex() ) {
                var rowStart = ( rowIdx * rowPortion ) * 2f - 1f;
                var rowEnd = ( ( rowIdx + 1 ) * rowPortion ) * 2f - 1f;

                for( var i = 0; i < row.Count - 1; i++ ) {
                    count++;

                    var (leftTime, _leftColor) = row[i];
                    var (rightTime, _rightColor) = row[i + 1];

                    var leftPosition = ( leftTime - startTime ) / timeDiff * 2f - 1f;
                    var rightPosition = ( rightTime - startTime ) / timeDiff * 2f - 1f;
                    var leftColor = new Vector4( _leftColor.X, _leftColor.Y, _leftColor.Z, 1f );
                    var rightColor = new Vector4( _rightColor.X, _rightColor.Y, _rightColor.Z, 1f );

                    var topLeftPos = new Vector4( leftPosition, rowStart, 0, 1 );
                    var topRightPos = new Vector4( rightPosition, rowStart, 0, 1 );
                    var bottomLeftPos = new Vector4( leftPosition, rowEnd, 0, 1 );
                    var bottomRightPos = new Vector4( rightPosition, rowEnd, 0, 1 );

                    data.Add( topLeftPos );
                    data.Add( leftColor );

                    data.Add( topRightPos );
                    data.Add( rightColor );

                    data.Add( bottomRightPos );
                    data.Add( rightColor );

                    data.Add( topLeftPos );
                    data.Add( leftColor );

                    data.Add( bottomRightPos );
                    data.Add( rightColor );

                    data.Add( bottomLeftPos );
                    data.Add( leftColor );
                }
            }

            Gradient.SetVertexes( Device, [.. data], count * 6 );
            Draw();
        }

        private void RefreshRasterizeState() {
            State?.Dispose();
            State = new RasterizerState( Device, new RasterizerStateDescription {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            } );
        }

        private void ResizeResources() {
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

        public override void Draw() {
            BeforeDraw( out var oldState, out var oldRenderViews, out var oldDepthStencilView, out var oldDepthStencilState );

            Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
            Ctx.ClearRenderTargetView( RenderView, new RawColor4( 0.3f, 0.3f, 0.3f, 1.0f ) );
            Ctx.Rasterizer.SetViewport( 0, 0, Width, Height, 0.0f, 1.0f );
            Ctx.Rasterizer.State = State;

            Gradient.SetupPass( Ctx, PassType.Final );
            Gradient.DrawVertexes( Ctx );

            Ctx.Flush();

            AfterDraw( oldState, oldRenderViews, oldDepthStencilView, oldDepthStencilState );
        }

        public override void Dispose() {
            State?.Dispose();
            RenderTexture?.Dispose();
            ShaderView?.Dispose();
            RenderView?.Dispose();
            DepthTexture?.Dispose();
            DepthView?.Dispose();
            Gradient?.Dispose();
        }
    }
}
