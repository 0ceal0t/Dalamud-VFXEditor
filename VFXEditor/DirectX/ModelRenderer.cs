using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    public abstract class ModelRenderer : Renderer {
        public IntPtr Output => RenderShad.NativePointer;

        public bool IsWireframe = false;
        public bool IsDragging = false;

        private Vector2 LastMousePos;
        private float Yaw;
        private float Pitch;
        private Vector3 Position = new( 0, 0, 0 );
        private float Distance = 5;
        private Matrix LocalMatrix = Matrix.Identity;

        protected int Width = 300;
        protected int Height = 300;
        protected bool FirstModel = false;

        protected RasterizerState RState;
        protected Buffer RendersizeBuffer;
        protected Buffer WorldBuffer;
        protected Matrix ViewMatrix;
        protected Matrix ProjMatrix;
        protected Texture2D DepthTex;
        protected DepthStencilView DepthView;
        protected Texture2D RenderTex;
        protected ShaderResourceView RenderShad;
        protected RenderTargetView RenderView;

        public ModelRenderer( Device device, DeviceContext ctx ) : base( device, ctx ) {
            WorldBuffer = new Buffer( Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            ViewMatrix = Matrix.LookAtLH( new Vector3( 0, 0, -Distance ), Position, Vector3.UnitY );

            RendersizeBuffer = new Buffer( Device, Utilities.SizeOf<Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );

            RefreshRasterizeState();
            ResizeResources();
        }

        public void RefreshRasterizeState() {
            RState?.Dispose();
            RState = new RasterizerState( Device, new RasterizerStateDescription {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = IsWireframe ? FillMode.Wireframe : FillMode.Solid,
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
                Distance += mouseWheel * 0.2f;
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

        public void Draw() {
            BeforeDraw( out var oldState, out var oldRenderViews, out var oldDepthStencilView );

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = LocalMatrix * viewProj;
            worldViewProj.Transpose();
            Ctx.UpdateSubresource( ref worldViewProj, WorldBuffer );

            var renderSize = new Vector4( Width, Height, 0, 0 );
            Ctx.UpdateSubresource( ref renderSize, RendersizeBuffer );

            Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth, 1.0f, 0 );
            Ctx.ClearRenderTargetView( RenderView, new Color4( 0.3f, 0.3f, 0.3f, 1.0f ) );

            Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            Ctx.Rasterizer.State = RState;

            OnDraw();

            Ctx.Flush();

            AfterDraw( oldState, oldRenderViews, oldDepthStencilView );
        }

        public void DrawInline() {
            var cursor = ImGui.GetCursorScreenPos();
            using var child = ImRaii.Child( "3DChild" );

            var space = ImGui.GetContentRegionAvail();
            Resize( space );

            ImGui.ImageButton( Output, space, new Vec2( 0, 0 ), new Vec2( 1, 1 ), 0 );

            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) Zoom( ImGui.GetIO().MouseWheel );
        }

        public abstract void OnDispose();
        public void Dispose() {
            RState?.Dispose();
            RenderTex?.Dispose();
            RenderShad?.Dispose();
            RenderView?.Dispose();
            DepthTex?.Dispose();
            DepthView?.Dispose();
            WorldBuffer?.Dispose();
            RendersizeBuffer?.Dispose();

            OnDispose();
        }
    }
}
