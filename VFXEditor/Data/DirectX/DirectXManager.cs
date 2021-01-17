using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VFXEditor {
    public class DirectXManager {
        Plugin _plugin;
        public string ShaderPath;
        public bool DoRender = true;

        public int Width = 300;
        public int Height = 300;

        Device _Device;
        DeviceContext _Ctx;
        CompilationResult vertexShaderByteCode;
        CompilationResult pixelShaderByteCode;
        PixelShader PShader;
        VertexShader VShader;
        ShaderSignature Signature;
        InputLayout Layout;

        Buffer Vertices;
        Buffer WorldBuffer;

        Matrix ViewMatrix;
        Matrix ProjMatrix;

        Texture2D DepthTex;
        DepthStencilView DepthView;
        public Texture2D RenderTex;
        public ShaderResourceView RenderShad;
        public RenderTargetView RenderView;

        Stopwatch Clock;

        public DirectXManager(Plugin plugin) {
            _plugin = plugin;
            ShaderPath = Path.Combine( plugin.TemplateLocation, "Shaders" );

            _Device = new Device( DriverType.Hardware );
            _Ctx = _Device.ImmediateContext;
            // don't set up a swapchain b/c we don't need a window hehehehe.....

            // SET UP SHADERS
            var shaderFile = Path.Combine( ShaderPath, "ModelPreview.fx" );
            PluginLog.Log( "Shader: " + shaderFile );
            vertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
            pixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
            VShader = new VertexShader( _Device, vertexShaderByteCode );
            PShader = new PixelShader( _Device, pixelShaderByteCode );

            Signature = ShaderSignature.GetInputSignature( vertexShaderByteCode );
            Layout = new InputLayout( _Device, Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            } );

            // VERTEX BUFFER
            Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, new[]
            {
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
            } );

            // WORLD MATRIX BUFFER
            WorldBuffer = new Buffer( _Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            _Ctx.PixelShader.Set( PShader );
            _Ctx.VertexShader.Set( VShader );
            _Ctx.InputAssembler.InputLayout = Layout;
            _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer ); // set world buffer
            _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * 2, 0 ) ); // set vertex buffer

            ViewMatrix = Matrix.LookAtLH( new Vector3( 0, 0, -5 ), new Vector3( 0, 0, 0 ), Vector3.UnitY );
            ProjMatrix = Matrix.Identity;

            RenderTex = new Texture2D( _Device, new Texture2DDescription()
            {
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
            RenderShad = new ShaderResourceView( _Device, RenderTex );
            RenderView = new RenderTargetView( _Device, RenderTex );

            DepthTex = new Texture2D( _Device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription( 1, 0 ),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            } );
            DepthView = new DepthStencilView( _Device, DepthTex );

            _Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            _Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            ProjMatrix = Matrix.PerspectiveFovLH( ( float )Math.PI / 4.0f, Width / ( float )Height, 0.1f, 100.0f );

            Clock = new Stopwatch();
            Clock.Start();

            PluginLog.Log( "Set up DirectX" );
        }

        int i = 0;
        public void Draw() {
            if( !DoRender )
                return;

            var time = Clock.ElapsedMilliseconds / 1000.0f;

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = Matrix.RotationX( time ) * Matrix.RotationY( time * 2 ) * Matrix.RotationZ( time * .7f ) * viewProj;
            worldViewProj.Transpose();
            _Ctx.UpdateSubresource( ref worldViewProj, WorldBuffer );

            _Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
            _Ctx.ClearRenderTargetView( RenderView, Color.Green );

            _Ctx.Draw( 36, 0 );

            _Ctx.Flush();

            if( i < 10 ) {
                i++;
                if(i == 10)
                    Save();
            }
        }

        public void Save() {
            PluginLog.Log( "writing" );

            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
            var screenTexture = new Texture2D( _Device, textureDesc );
            _Ctx.CopyResource( RenderTex, screenTexture );

            var mapSource = _Ctx.MapSubresource( screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None );

            var bitmap = new System.Drawing.Bitmap( Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
            var boundsRect = new System.Drawing.Rectangle( 0, 0, Width, Height );
            var mapDest = bitmap.LockBits( boundsRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat );
            var sourcePtr = mapSource.DataPointer;
            var destPtr = mapDest.Scan0;
            for( int y = 0; y < Height; y++ ) {
                Utilities.CopyMemory( destPtr, sourcePtr, Width * 4 );
                sourcePtr = IntPtr.Add( sourcePtr, mapSource.RowPitch );
                destPtr = IntPtr.Add( destPtr, mapDest.Stride );
            }
            bitmap.UnlockBits( mapDest );
            _Ctx.UnmapSubresource( screenTexture, 0 );
            bitmap.Save( @"C:\Users\kamin\Downloads\r\test.bmp" );
        }

        public void Dispose() {
            DoRender = false;

            RenderTex?.Dispose();
            RenderShad?.Dispose();
            RenderView?.Dispose();
            DepthTex?.Dispose();
            DepthView?.Dispose();
            WorldBuffer?.Dispose();
            Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            vertexShaderByteCode?.Dispose();
            pixelShaderByteCode?.Dispose();
            _Ctx.ClearState();
            _Ctx.Flush();
            _Device?.Dispose();
            _Ctx?.Dispose();
        }
    }
}
