using Dalamud.Logging;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using VfxEditor.AvfxFormat;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class GradientView : Renderer {
        public AvfxCurve CurrentCurve { get; private set; }
        public IntPtr Output => RenderShad.NativePointer;

        private readonly int Width = 500;
        private readonly int Height = 50;

        private RasterizerState RState;
        private Texture2D DepthTex;
        private DepthStencilView DepthView;
        private Texture2D RenderTex;
        private ShaderResourceView RenderShad;
        private RenderTargetView RenderView;

        private static readonly int ModelSpan = 2; // position, color
        private int NumVerts;
        private Buffer Vertices;
        private readonly CompilationResult VertexShaderByteCode;
        private readonly CompilationResult PixelShaderByteCode;
        private readonly PixelShader PShader;
        private readonly VertexShader VShader;
        private readonly ShaderSignature Signature;
        private readonly InputLayout Layout;

        public readonly bool ShaderError = false;

        public GradientView( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            RefreshRasterizeState();
            ResizeResources();

            NumVerts = 0;
            Vertices = null;

            try {
                var shaderFile = Path.Combine( shaderPath, "Gradient.fx" );
                VertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
                PixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
                VShader = new VertexShader( Device, VertexShaderByteCode );
                PShader = new PixelShader( Device, PixelShaderByteCode );
                Signature = ShaderSignature.GetInputSignature( VertexShaderByteCode );
                Layout = new InputLayout( Device, Signature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                } );
            }
            catch( Exception e ) {
                PluginLog.Error( "Error compiling shaders", e );
                ShaderError = true;
            }
        }

        public void SetGradient( AvfxCurve curve ) {
            CurrentCurve = curve;
            var numPoints = curve.Keys.Keys.Count;
            if( numPoints < 2 ) {
                NumVerts = 0;
                Vertices?.Dispose();
            }
            else {
                // each set of 2 keys needs 6 points
                var data = new Vector4[( numPoints - 1 ) * 6 * ModelSpan];
                float startTime = curve.Keys.Keys[0].Time;
                float endTime = curve.Keys.Keys[numPoints - 1].Time;
                var timeDiff = ( endTime - startTime );

                for( var i = 0; i < numPoints - 1; i++ ) {
                    var left = curve.Keys.Keys[i];
                    var right = curve.Keys.Keys[i + 1];

                    var leftPosition = ( ( left.Time - startTime ) / timeDiff ) * 2 - 1;
                    var rightPosition = ( ( right.Time - startTime ) / timeDiff ) * 2 - 1;
                    var leftColor = new Vector4( left.X, left.Y, left.Z, 1 );
                    var rightColor = new Vector4( right.X, right.Y, right.Z, 1 );

                    var topLeft = new Vector4( leftPosition, 1, 0, 1 );
                    var topRight = new Vector4( rightPosition, 1, 0, 1 );
                    var bottomLeft = new Vector4( leftPosition, -1, 0, 1 );
                    var bottomRight = new Vector4( rightPosition, -1, 0, 1 );

                    var idx = i * 6 * ModelSpan;
                    data[idx + 0] = topLeft;
                    data[idx + 1] = leftColor;

                    data[idx + 2] = topRight;
                    data[idx + 3] = rightColor;

                    data[idx + 4] = bottomRight;
                    data[idx + 5] = rightColor;

                    data[idx + 6] = topLeft;
                    data[idx + 7] = leftColor;

                    data[idx + 8] = bottomRight;
                    data[idx + 9] = rightColor;

                    data[idx + 10] = bottomLeft;
                    data[idx + 11] = leftColor;
                }
                Vertices?.Dispose();
                Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                NumVerts = ( numPoints - 1 ) * 6;
            }
            Draw();
        }

        private void RefreshRasterizeState() {
            RState?.Dispose();
            RState = new RasterizerState( Device, new RasterizerStateDescription {
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
            RenderTex?.Dispose();
            RenderTex = new Texture2D( Device, new Texture2DDescription() {
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
            RenderShad?.Dispose();
            RenderShad = new ShaderResourceView( Device, RenderTex );
            RenderView?.Dispose();
            RenderView = new RenderTargetView( Device, RenderTex );

            DepthTex?.Dispose();
            DepthTex = new Texture2D( Device, new Texture2DDescription() {
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
            DepthView = new DepthStencilView( Device, DepthTex );
        }

        public void Draw() {
            BeforeDraw( out var oldState, out var oldRenderViews, out var oldDepthStencilView );

            if( !ShaderError ) {
                Ctx.OutputMerger.SetTargets( DepthView, RenderView );
                Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
                Ctx.ClearRenderTargetView( RenderView, new Color4( 0.3f, 0.3f, 0.3f, 1.0f ) );

                Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
                Ctx.Rasterizer.State = RState;

                if( NumVerts > 0 ) {
                    Ctx.PixelShader.Set( PShader );
                    Ctx.VertexShader.Set( VShader );
                    Ctx.InputAssembler.InputLayout = Layout;
                    Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * ModelSpan, 0 ) );
                    Ctx.Draw( NumVerts, 0 );
                }

                Ctx.Flush();
            }

            AfterDraw( oldState, oldRenderViews, oldDepthStencilView );
        }

        public void Dispose() {
            RState?.Dispose();
            RenderTex?.Dispose();
            RenderShad?.Dispose();
            RenderView?.Dispose();
            DepthTex?.Dispose();
            DepthView?.Dispose();

            Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            VertexShaderByteCode?.Dispose();
            PixelShaderByteCode?.Dispose();
        }
    }
}
