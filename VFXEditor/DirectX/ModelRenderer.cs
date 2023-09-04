using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    [StructLayout( LayoutKind.Sequential )]
    public struct VSBufferStruct {
        public Matrix World;
        public Matrix ViewProjection;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PSBufferStruct {
        public Vector4 LightDirection;
        public Vector4 LightColor;
        public int ShowEdges;
        public Vector3 Padding;
    }

    public abstract class ModelRenderer : Renderer {
        public IntPtr Output => RenderShad.NativePointer;
        public bool IsDragging = false;

        private Vector2 LastMousePos;
        private float Yaw;
        private float Pitch;
        private Vector3 Position = new( 0, 0, 0 );
        private float Distance = 5;
        protected Matrix LocalMatrix = Matrix.Scaling( new Vector3( -1, 1, 1 ) );

        protected int Width = 300;
        protected int Height = 300;
        protected Matrix ViewMatrix;
        protected bool FirstModel = false;

        protected RasterizerState RasterizeState;
        protected Buffer VSBuffer;
        protected Buffer PSBuffer;
        protected Matrix ProjMatrix;

        protected Texture2D RenderTex;
        protected ShaderResourceView RenderShad;
        protected RenderTargetView RenderView;

        protected Texture2D DepthTex;
        protected DepthStencilView DepthView;

        public Vec2 LastSize = new( 100, 100 );
        public Vec2 LastMid = new( 50, 50 );

        public ModelRenderer( Device device, DeviceContext ctx ) : base( device, ctx ) {
            VSBuffer = new Buffer( Device, Utilities.SizeOf<VSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            PSBuffer = new Buffer( Device, Utilities.SizeOf<PSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            ViewMatrix = Matrix.LookAtLH( new Vector3( 0, 0, -Distance ), Position, Vector3.UnitY );
            RefreshRasterizeState();
            ResizeResources();
        }

        protected virtual bool Wireframe() => Plugin.Configuration.ModelWireframe;

        public void RefreshRasterizeState() {
            RasterizeState?.Dispose();
            RasterizeState = new RasterizerState( Device, new RasterizerStateDescription {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = Wireframe() ? FillMode.Wireframe : FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            } );
        }

        public void Resize( Vec2 size ) {
            var width = ( int )size.X;
            var height = ( int )size.Y;
            if( width != Width || height != Height ) {
                Width = width;
                Height = height;
                ResizeResources();
                if( FirstModel ) Draw();
            }
        }

        protected static int GetIdx( int faceIdx, int pointIdx, int span, int pointsPer ) => span * ( faceIdx * pointsPer + pointIdx );

        protected void ResizeResources() {
            ProjMatrix = Matrix.PerspectiveFovLH( ( float )Math.PI / 4.0f, Width / ( float )Height, 0.1f, 100.0f );

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
                OptionFlags = ResourceOptionFlags.None
            } );

            DepthView?.Dispose();
            DepthView = new DepthStencilView( Device, DepthTex );
        }

        protected static float Clamp( float value, float min, float max ) => value > max ? max : value < min ? min : value;

        public void Drag( Vec2 newPos, bool rotate ) {
            if( IsDragging ) {
                if( rotate ) {
                    Yaw += ( newPos.X - LastMousePos.X ) * 0.01f;
                    Pitch -= ( newPos.Y - LastMousePos.Y ) * 0.01f;
                    Pitch = Clamp( Pitch, -1.55f, 1.55f );
                }
                else {
                    Position.Y += ( newPos.Y - LastMousePos.Y ) * 0.01f;
                }
                UpdateViewMatrix();
            }
            IsDragging = true;
            LastMousePos = new Vector2( newPos.X, newPos.Y );
        }

        public void Zoom( float mouseWheel ) {
            if( mouseWheel != 0 ) {
                Distance -= mouseWheel * 0.2f;
                if( Distance < 0.001f ) Distance = 0.001f;
                UpdateViewMatrix();
            }
        }

        public void UpdateViewMatrix() {
            var lookRotation = Quaternion.RotationYawPitchRoll( Yaw, Pitch, 0f );
            var lookDirection = Vector3.Transform( -Vector3.UnitZ, lookRotation );
            ViewMatrix = Matrix.LookAtLH( Position - Distance * lookDirection, Position, Vector3.UnitY );
            Draw();
        }

        public void UpdateDraw() {
            if( !FirstModel ) {
                FirstModel = true;
                UpdateViewMatrix();
            }
            else {
                Draw();
            }
        }

        public abstract void OnDraw();

        protected virtual bool ShowEdges() => Plugin.Configuration.ModelShowEdges && !Plugin.Configuration.ModelWireframe;

        public void Draw() {
            BeforeDraw( out var oldState, out var oldRenderViews, out var oldDepthStencilView );

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var world = LocalMatrix;

            world.Transpose();
            viewProj.Transpose();

            var vsBuffer = new VSBufferStruct {
                World = world,
                ViewProjection = viewProj
            };

            var psBuffer = new PSBufferStruct {
                LightDirection = new( 1.0f, 1.0f, 1.0f, 1.0f ),
                LightColor = new( 10.0f, 8.0f, 5.0f, 0.0f ),
                ShowEdges = ShowEdges() ? 1 : 0,
            };

            Ctx.UpdateSubresource( ref vsBuffer, VSBuffer );
            Ctx.UpdateSubresource( ref psBuffer, PSBuffer );

            Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0 );
            Ctx.ClearRenderTargetView( RenderView, new Color4( 0.272f, 0.273f, 0.320f, 1.0f ) );

            Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            Ctx.Rasterizer.State = RasterizeState;

            OnDraw();

            Ctx.Flush();

            AfterDraw( oldState, oldRenderViews, oldDepthStencilView );
        }

        public virtual void DrawInline() {
            using var child = ImRaii.Child( "3DChild" );
            DrawImage();
        }

        protected virtual bool CanDrag() => true;

        protected void DrawImage() {
            var cursor = ImGui.GetCursorScreenPos();
            var size = ImGui.GetContentRegionAvail();
            Resize( size );

            ImGui.ImageButton( Output, size, new Vec2( 0, 0 ), new Vec2( 1, 1 ), 0 );

            var topLeft = ImGui.GetItemRectMin();
            var bottomRight = ImGui.GetItemRectMax();

            if( CanDrag() && ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                Drag( delta, true );
            }
            else if( CanDrag() && ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) Zoom( ImGui.GetIO().MouseWheel );

            LastSize = bottomRight - topLeft;
            LastMid = topLeft + ( LastSize / 2f );
        }

        public abstract void OnDispose();
        public void Dispose() {
            RasterizeState?.Dispose();
            RenderTex?.Dispose();
            RenderShad?.Dispose();
            RenderView?.Dispose();
            DepthTex?.Dispose();
            DepthView?.Dispose();
            VSBuffer?.Dispose();
            PSBuffer?.Dispose();

            OnDispose();
        }
    }
}
