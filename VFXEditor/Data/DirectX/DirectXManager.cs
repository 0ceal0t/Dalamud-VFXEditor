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
using System.Reflection;
using AVFXLib.Models;

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

        RasterizerState RState;

        Buffer Vertices;
        int NumVerts = 36;
        Buffer WorldBuffer;

        Matrix ViewMatrix;
        Matrix ProjMatrix;

        Texture2D DepthTex;
        DepthStencilView DepthView;
        public Texture2D RenderTex;
        public ShaderResourceView RenderShad;
        public RenderTargetView RenderView;

        Stopwatch Clock;

        // https://github.com/ackwell/BrowserHost/blob/32104cedd10715ca44710be1e57a36b6aa8c43c3/BrowserHost.Plugin/DxHandler.cs

        public DirectXManager(Plugin plugin) {
            _plugin = plugin;
            ShaderPath = Path.Combine( plugin.TemplateLocation, "Shaders" );

            //_Device = new Device( DriverType.Hardware );
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var dalamud = typeof( DalamudPluginInterface ).GetField( "dalamud", bindingFlags ).GetValue( _plugin.PluginInterface );
            var interfaceManager = dalamud.GetType().GetProperty( "InterfaceManager", bindingFlags ).GetValue( dalamud );
            var scene = interfaceManager.GetType().GetField( "scene", bindingFlags ).GetValue( interfaceManager );
            var sceneType = scene.GetType();
            _Device = ( Device )sceneType.GetField( "device", bindingFlags ).GetValue( scene );
            _Ctx = _Device.ImmediateContext;
            // don't set up a swapchain b/c we don't need a window hehehehe.....

            // SET UP SHADERS
            var shaderFile = Path.Combine( ShaderPath, "ModelPreview.fx" );
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
            ViewMatrix = Matrix.LookAtLH( new Vector3( 0, 0, -5 ), new Vector3( 0, 0, 0 ), Vector3.UnitY );
            ProjMatrix = Matrix.PerspectiveFovLH( ( float )Math.PI / 4.0f, Width / ( float )Height, 0.1f, 100.0f );

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

            RState = new RasterizerState( _Device, new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            } );

            Clock = new Stopwatch();
            Clock.Start();

            PluginLog.Log( "Set up DirectX" );
        }

        public void LoadModel(AVFXModel model) {
            if( model.Indexes.Count == 0 ) {
                NumVerts = 0;
                Vertices.Dispose();
                return;
            }

            Vector4[] data = new Vector4[model.Indexes.Count * 3 * 2]; // 3 vertices per index, 2 vectors per vertex
            for(int index = 0; index < model.Indexes.Count; index++ ) {
                var _index = model.Indexes[index];
                var v1 = model.Vertices[_index.I1];
                var v2 = model.Vertices[_index.I2];
                var v3 = model.Vertices[_index.I3];

                var idx = index * 6;
                data[idx + 0] = new Vector4( v1.Position[0], v1.Position[1], v1.Position[2], 1.0f );
                data[idx + 1] = new Vector4( v1.Color[0] / 255, v1.Color[1] / 255, v1.Color[2] / 255, 1.0f );
                data[idx + 2] = new Vector4( v2.Position[0], v2.Position[1], v2.Position[2], 1.0f );
                data[idx + 3] = new Vector4( v2.Color[0] / 255, v2.Color[1] / 255, v2.Color[2] / 255, 1.0f );
                data[idx + 4] = new Vector4( v3.Position[0], v3.Position[1], v3.Position[2], 1.0f );
                data[idx + 5] = new Vector4( v3.Color[0] / 255, v3.Color[1] / 255, v3.Color[2] / 255, 1.0f );
            }
            //_Ctx.UpdateSubresource( data, Vertices );
            Vertices.Dispose();
            Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, data );
            NumVerts = data.Length;
        }

        public void Draw() {
            if( !_plugin.MainUI.Visible ) // no need to render
                return;
            // TODO: check if models tab open

            _Ctx.PixelShader.Set( PShader );
            _Ctx.VertexShader.Set( VShader );
            _Ctx.InputAssembler.InputLayout = Layout;
            _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer ); // set world buffer

            var time = Clock.ElapsedMilliseconds / 1000.0f;

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = Matrix.RotationX( time ) * Matrix.RotationY( time * 2 ) * Matrix.RotationZ( time * .7f ) * viewProj;
            worldViewProj.Transpose();
            _Ctx.UpdateSubresource( ref worldViewProj, WorldBuffer );

            _Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            var oldCull = _Ctx.Rasterizer.State;
            _Ctx.Rasterizer.State = RState;

            var oldRenderViews = _Ctx.OutputMerger.GetRenderTargets( OutputMergerStage.SimultaneousRenderTargetCount, out var oldDepthStencilView );
            _Ctx.OutputMerger.SetTargets( DepthView, RenderView );

            _Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
            _Ctx.ClearRenderTargetView( RenderView, Color.Green );

            if( NumVerts > 0 ) {
                _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * 2, 0 ) ); // set vertex buffer
                _Ctx.Draw( NumVerts, 0 );
            }
            _Ctx.Flush();

            _Ctx.Rasterizer.State = oldCull;
            _Ctx.OutputMerger.SetRenderTargets( oldDepthStencilView, oldRenderViews );
        }

        public void Save() {
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
            bitmap.Save( @"test.bmp" );
        }

        public void Dispose() {
            DoRender = false;
            Clock.Stop();

            RState?.Dispose();
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
            _Device = null;
            _Ctx = null;
        }
    }
}
