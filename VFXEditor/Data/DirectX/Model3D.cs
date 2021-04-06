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
using Vec2 = System.Numerics.Vector2;

namespace VFXEditor.Data.DirectX {
    public class Model3D {
        public DirectXManager Manager;
        public Device _Device;
        public DeviceContext _Ctx;

        public int Width = 300;
        public int Height = 300;

        RasterizerState RState;
        Buffer WorldBuffer;
        Matrix ViewMatrix;
        Matrix ProjMatrix;
        Texture2D DepthTex;
        DepthStencilView DepthView;
        public Texture2D RenderTex;
        public ShaderResourceView RenderShad;
        public RenderTargetView RenderView;

        // ====== BASE MODEL ========
        int NumVerts;
        Buffer Vertices;
        CompilationResult vertexShaderByteCode;
        CompilationResult pixelShaderByteCode;
        PixelShader PShader;
        VertexShader VShader;
        ShaderSignature Signature;
        InputLayout Layout;

        // ======= MODEL EDGE ========
        int EDGE_NumVerts;
        Buffer EDGE_Vertices;
        CompilationResult EDGE_VertexShaderByteCode;
        CompilationResult EDGE_PixelShaderByteCode;
        PixelShader EDGE_PShader;
        VertexShader EDGE_VShader;
        ShaderSignature EDGE_Signature;
        InputLayout EDGE_Layout;

        public bool IsDragging = false;
        private Vector2 LastMousePos;
        private float Yaw;
        private float Pitch;
        private Vector3 Position = new Vector3( 0, 0, 0 );
        private float Distance = 5;

        public bool IsWireframe = false;
        public bool ShowEdges = true;

        public Model3D(DirectXManager manager) {
            Manager = manager;
            _Device = Manager._Device;
            _Ctx = Manager._Ctx;

            // ======= BASE MODEL =========
            var shaderFile = Path.Combine( Manager.ShaderPath, "ModelPreview.fx" );
            vertexShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "VS", "vs_4_0" );
            pixelShaderByteCode = ShaderBytecode.CompileFromFile( shaderFile, "PS", "ps_4_0" );
            VShader = new VertexShader( _Device, vertexShaderByteCode );
            PShader = new PixelShader( _Device, pixelShaderByteCode );
            Signature = ShaderSignature.GetInputSignature( vertexShaderByteCode );
            Layout = new InputLayout( _Device, Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
            } );
            NumVerts = 0;
            Vertices = null;

            // ======= MODEL EDGES ========
            var EDGE_shaderFile = Path.Combine( Manager.ShaderPath, "ModelEdge.fx" );
            EDGE_VertexShaderByteCode = ShaderBytecode.CompileFromFile( EDGE_shaderFile, "VS", "vs_4_0" );
            EDGE_PixelShaderByteCode = ShaderBytecode.CompileFromFile( EDGE_shaderFile, "PS", "ps_4_0" );
            EDGE_VShader = new VertexShader( _Device, EDGE_VertexShaderByteCode );
            EDGE_PShader = new PixelShader( _Device, EDGE_PixelShaderByteCode );
            EDGE_Signature = ShaderSignature.GetInputSignature( EDGE_VertexShaderByteCode );
            EDGE_Layout = new InputLayout( _Device, EDGE_Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            } );
            EDGE_NumVerts = 0;
            EDGE_Vertices = null;

            // WORLD MATRIX BUFFER
            WorldBuffer = new Buffer( _Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            ViewMatrix = Matrix.LookAtLH( new Vector3(0, 0, -Distance), Position, Vector3.UnitY );

            RefreshRasterizeState();
            ResizeResources();
        }

        public void RefreshRasterizeState() {
            RState?.Dispose();
            RState = new RasterizerState( _Device, new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = IsWireframe ? FillMode.Wireframe : FillMode.Solid,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            } );
        }

        public void Resize( Vec2 size ) {
            var w_ = ( int )size.X;
            var h_ = ( int )size.Y;
            if( w_ != Width || h_ != Height ) {
                Width = w_;
                Height = h_;
                ResizeResources();
            }
        }

        public void ResizeResources() {
            ProjMatrix = Matrix.PerspectiveFovLH( ( float )Math.PI / 4.0f, Width / ( float )Height, 0.1f, 100.0f );
            RenderTex?.Dispose();
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
            RenderShad?.Dispose();
            RenderShad = new ShaderResourceView( _Device, RenderTex );
            RenderView?.Dispose();
            RenderView = new RenderTargetView( _Device, RenderTex );

            DepthTex?.Dispose();
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
            DepthView?.Dispose();
            DepthView = new DepthStencilView( _Device, DepthTex );
        }

        private float Clamp( float value, float min, float max ) {
            return value > max
                ? max
                : value < min
                    ? min
                    : value;
        }
        public void Drag(Vec2 newPos ) {
            if( IsDragging ) {
                Yaw += ( newPos.X - LastMousePos.X ) * 0.01f;
                Pitch += -( newPos.Y - LastMousePos.Y ) * 0.01f;
                Pitch = Clamp( Pitch, -1.55f, 1.55f );
                UpdateViewMatrix();
            }
            IsDragging = true;
            LastMousePos = new Vector2( newPos.X, newPos.Y );
        }
        public void Zoom(float mouseWheel ) {
            if(mouseWheel != 0 ) {
                Distance += mouseWheel * 0.2f;
                UpdateViewMatrix();
            }
        }
        public void UpdateViewMatrix() {
            var lookRotation = Quaternion.RotationYawPitchRoll( Yaw, Pitch, 0f );
            Vector3 lookDirection = Vector3.Transform( -Vector3.UnitZ, lookRotation );
            ViewMatrix = Matrix.LookAtLH( Position - Distance * lookDirection, Position, Vector3.UnitY );
        }

        public static int MODEL_SPAN = 3; // position, color, normal
        public static int EDGE_SPAN = 2;
        public static Vector4 LINE_COLOR = new Vector4( 1, 0, 0, 1 );

        public void LoadModel( AVFXModel model, int mode = 1 ) {
            if( model.Indexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();

                EDGE_NumVerts = 0;
                EDGE_Vertices?.Dispose();
                return;
            }

            Vector4[] data = new Vector4[model.Indexes.Count * 3 * MODEL_SPAN]; // 3 vertices per face
            Vector4[] EDGE_data = new Vector4[model.Indexes.Count * 4 * EDGE_SPAN]; // 4 points per loop

            for( int index = 0; index < model.Indexes.Count; index++ ) { // each face
                int[] _indexes = new int[] { model.Indexes[index].I1, model.Indexes[index].I2, model.Indexes[index].I3 };
                for(int j = 0; j < _indexes.Length; j++ ) { // push all 3 vertices per face
                    var idx = GetIdx( index, j, MODEL_SPAN, 3 );
                    var v = model.Vertices[_indexes[j]];

                    data[idx + 0] = new Vector4( v.Position[0], v.Position[1], v.Position[2], 1.0f );
                    if(mode == 1 ) { // COLOR
                        data[idx + 1] = new Vector4( v.Color[0] / 255, v.Color[1] / 255, v.Color[2] / 255, 1.0f );
                    }
                    else if(mode == 2 ) { // UV1
                        data[idx + 1] = new Vector4( v.UV1[0] + 0.5f, 0, v.UV1[2] + 0.5f, 1.0f );
                    }
                    else if( mode == 3 ) { // UV2
                        data[idx + 1] = new Vector4( v.UV2[2] + 0.5f, 0, v.UV2[3] + 0.5f, 1.0f );
                    }
                    data[idx + 2] = new Vector4( v.Normal[0], v.Normal[1], v.Normal[2], 1.0f );

                    // ========= COPY OVER EDGE DATA ==========
                    int edgeIdx = GetIdx( index, j, EDGE_SPAN, 4 );
                    EDGE_data[edgeIdx + 0] = data[idx + 0];
                    EDGE_data[edgeIdx + 1] = LINE_COLOR;
                    if(j == 0 ) { // loop back around
                        int lastEdgeIdx = GetIdx( index, 3, EDGE_SPAN, 4 );
                        EDGE_data[lastEdgeIdx + 0] = data[idx + 0];
                        EDGE_data[lastEdgeIdx + 1] = LINE_COLOR;
                    }
                }
            }

            Vertices?.Dispose();
            Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, data );
            NumVerts = model.Indexes.Count * 3;

            EDGE_Vertices?.Dispose();
            EDGE_Vertices = Buffer.Create( _Device, BindFlags.VertexBuffer, EDGE_data );
            EDGE_NumVerts = model.Indexes.Count * 4;
        }

        public static int GetIdx(int faceIdx, int pointIdx, int span, int pointsPer ) {
            return span * ( faceIdx * pointsPer + pointIdx );
        }

        public void Draw() {
            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = Matrix.Identity * viewProj;
            worldViewProj.Transpose();
            _Ctx.UpdateSubresource( ref worldViewProj, WorldBuffer );

            _Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            _Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
            _Ctx.ClearRenderTargetView( RenderView, new Color4( 0.3f, 0.3f, 0.3f, 1.0f ) );

            _Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            _Ctx.Rasterizer.State = RState;

            // ====== DRAW BASE =========
            _Ctx.PixelShader.Set( PShader );
            _Ctx.VertexShader.Set( VShader );
            _Ctx.InputAssembler.InputLayout = Layout;
            _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
            if( NumVerts > 0 ) {
                _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * MODEL_SPAN, 0 ) ); // set vertex buffer
                _Ctx.Draw( NumVerts, 0 );
            }

            // ====== DRAW LINE =========
            if( ShowEdges ) {
                _Ctx.PixelShader.Set( EDGE_PShader );
                _Ctx.VertexShader.Set( EDGE_VShader );
                _Ctx.InputAssembler.InputLayout = EDGE_Layout;
                _Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
                _Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );
                if( EDGE_NumVerts > 0 ) {
                    _Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( EDGE_Vertices, Utilities.SizeOf<Vector4>() * EDGE_SPAN, 0 ) ); // set vertex buffer
                    for( int i = 0; i < EDGE_NumVerts / 4; i++ ) {
                        _Ctx.Draw( 4, i * 4 );
                    }
                }
            }

            _Ctx.Flush();
        }

        public void Dispose() {
            Vertices?.Dispose();
            EDGE_Vertices?.Dispose();

            RState?.Dispose();
            RenderTex?.Dispose();
            RenderShad?.Dispose();
            RenderView?.Dispose();
            DepthTex?.Dispose();
            DepthView?.Dispose();
            WorldBuffer?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            vertexShaderByteCode?.Dispose();
            pixelShaderByteCode?.Dispose();
        }
    }
}
