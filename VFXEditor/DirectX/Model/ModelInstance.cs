using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using HelixToolkit.Maths;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Numerics;
using VfxEditor;
using VfxEditor.Utils;

namespace VfxEditor.DirectX.Model {
    public class ModelInstance : RenderInstance {
        public nint Output => RenderResource.NativePointer;

        public float Yaw { get; protected set; }
        public float Pitch { get; protected set; }
        public Vector3 EyePosition { get; protected set; } = new( 0, 0, 0 );
        public Vector3 CameraPosition { get; protected set; }
        public float Distance { get; protected set; } = 5;

        public Matrix4x4 LocalMatrix { get; protected set; } = MatrixHelper.Scaling( new Vector3( -1, 1, 1 ) );

        public int Width { get; protected set; } = 300;
        public int Height { get; protected set; } = 300;

        public Matrix4x4 ViewMatrix { get; protected set; }
        public Matrix4x4 ProjMatrix { get; protected set; }
        public Matrix4x4 CubeMatrix { get; protected set; }

        public Texture2D RenderTexture { get; protected set; }
        public ShaderResourceView RenderResource { get; protected set; }
        public RenderTargetView RenderTarget { get; protected set; }
        public Texture2D StencilTexture { get; protected set; }
        public DepthStencilView StencilView { get; protected set; }

        protected Vector2 LastMousePos;
        protected bool IsDragging = false;

        protected Vector2 LastSize = new( 100, 100 );
        protected Vector2 LastMid = new( 50, 50 );

        public ModelInstance() {
            UpdateViewMatrix();
            ResizeResources();
        }

        public bool Resize( Vector2 size ) {
            var width = ( int )size.X;
            var height = ( int )size.Y;
            if( width != Width || height != Height ) {
                Width = width;
                Height = height;
                ResizeResources();
                return true;
            }
            return false;
        }

        protected override void ResizeResources() {
            ProjMatrix = MatrixHelper.PerspectiveFovLH( ( float )Math.PI / 4.0f, Width / ( float )Height, 0.1f, 100.0f );

            RenderTexture?.Dispose();
            RenderTexture = new Texture2D( Device, new() {
                Format = Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new( 1, 0 ),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            } );

            RenderResource?.Dispose();
            RenderResource = new ShaderResourceView( Device, RenderTexture );

            RenderTarget?.Dispose();
            RenderTarget = new RenderTargetView( Device, RenderTexture );

            // ====== DEPTH ==========

            StencilTexture?.Dispose();
            StencilTexture = new Texture2D( Device, new() {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32_Typeless,
                Width = Width,
                Height = Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new( 1, 0 ),
                Usage = ResourceUsage.Default
            } );

            StencilView?.Dispose();
            StencilView = new DepthStencilView( Device, StencilTexture, new() {
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = Format.D32_Float,
            } );
        }

        public virtual void DrawInstanceTexture( Action? drawPopup ) {
            DrawImage( drawPopup );
        }

        public void DrawImage( Action? drawPopup ) {
            var cursor = ImGui.GetCursorScreenPos();
            var size = ImGui.GetContentRegionAvail();

            var pos = ImGui.GetCursorScreenPos();
            ImGui.ImageButton( new ImTextureID( Output ), size, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );

            var topLeft = ImGui.GetItemRectMin();
            var bottomRight = ImGui.GetItemRectMax();
            LastSize = bottomRight - topLeft;
            LastMid = topLeft + LastSize / 2f;

            // ==== BUTTONS ========

            var topRight = pos + new Vector2( LastSize.X - 5, 5 );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( DrawButton( FontAwesomeIcon.Cog.ToIconString(), new Vector2( 25, 25 ), topRight - new Vector2( 25, 0 ) ) ) ImGui.OpenPopup( "Popup" );
            }

            topRight += new Vector2( -30, 0 );

            if( DrawButton( "Reset", new Vector2( 43, 25 ), topRight - new Vector2( 43, 0 ) ) ) {
                LastMousePos = default;
                Yaw = default;
                Pitch = default;
                EyePosition = new( 0, 0, 0 );
                Distance = 5;
                UpdateViewMatrix();
            }

            using( var popup = ImRaii.Popup( "Popup" ) ) {
                if( popup ) {
                    Plugin.Configuration.DrawDirectXCommon();
                    drawPopup?.Invoke();
                }
            }

            // ================

            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                Drag( ImGui.GetMouseDragDelta(), true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) Zoom( ImGui.GetIO().MouseWheel );
        }

        private void Zoom( float mouseWheel ) {
            if( mouseWheel != 0 ) {
                Distance -= mouseWheel * 0.2f;
                if( Distance < 0.001f ) Distance = 0.001f;
                UpdateViewMatrix();
            }
        }

        private void Drag( Vector2 newPos, bool rotate ) {
            if( IsDragging ) {
                if( rotate ) {
                    Yaw += ( newPos.X - LastMousePos.X ) * 0.01f;
                    Pitch -= ( newPos.Y - LastMousePos.Y ) * 0.01f;
                    Pitch = Clamp( Pitch, -1.55f, 1.55f );
                }
                else {
                    EyePosition = EyePosition with {
                        Y = EyePosition.Y + ( newPos.Y - LastMousePos.Y ) * 0.01f
                    };
                }
                UpdateViewMatrix();
            }
            IsDragging = true;
            LastMousePos = new Vector2( newPos.X, newPos.Y );
        }

        protected static float Clamp( float value, float min, float max ) => value > max ? max : value < min ? min : value;

        private static bool DrawButton( string text, Vector2 size, Vector2 topLeft ) {
            var drawList = ImGui.GetWindowDrawList();
            var hovered = UiUtils.MouseOver( topLeft, topLeft + size );
            drawList.AddRectFilled( topLeft, topLeft + size, ImGui.GetColorU32( hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button ), ImGui.GetStyle().FrameRounding );
            drawList.AddText( topLeft + ImGui.GetStyle().FramePadding, ImGui.GetColorU32( ImGuiCol.Text ), text );
            if( hovered && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) return true;
            return false;
        }

        private void UpdateViewMatrix() {
            var lookRotation = QuaternionHelper.RotationYawPitchRoll( Yaw, Pitch, 0f );
            var lookDirection = Vector3.Transform( -Vector3.UnitZ, lookRotation );
            CameraPosition = EyePosition - Distance * lookDirection;

            ViewMatrix = MatrixHelper.LookAtLH( CameraPosition, EyePosition, Vector3.UnitY );
            CubeMatrix = MatrixHelper.LookAtLH( new Vector3( 0 ) - 1 * lookDirection, new Vector3( 0 ), Vector3.UnitY );

            NeedsRender = true;
        }

        public override void Dispose() {
            base.Dispose();

            RenderTexture?.Dispose();
            RenderResource?.Dispose();
            RenderTarget?.Dispose();

            StencilTexture?.Dispose();
            StencilView?.Dispose();
        }
    }
}
