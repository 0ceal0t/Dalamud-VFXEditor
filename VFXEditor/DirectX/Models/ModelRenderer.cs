using Dalamud.Interface.Utility.Raii;
using HelixToolkit.SharpDX.Core;
using ImGuiNET;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.DirectX.Drawable;
using VfxEditor.Utils;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    [StructLayout( LayoutKind.Sequential )]
    public struct VSBufferStruct {
        public Matrix ModelMatrix;
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
        public Matrix ViewProjectionMatrix;
        public Matrix NormalMatrix;
        public Matrix CubeMatrix;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PSBufferStruct {
        public int ShowEdges;
        public Vector3 _Pad0;
    }

    public abstract class ModelRenderer : Renderer {
        public IntPtr Output => RenderResource.NativePointer;
        public bool IsDragging = false;
        protected bool NeedsRedraw = false;

        private Vector2 LastMousePos;
        private float Yaw;
        private float Pitch;
        private Vector3 Position = new( 0, 0, 0 );
        protected Vector3 CameraPosition;
        private float Distance = 5;
        protected Matrix LocalMatrix = Matrix.Scaling( new Vector3( -1, 1, 1 ) );

        protected int Width = 300;
        protected int Height = 300;
        protected bool FirstModel = false;

        protected Matrix ViewMatrix;
        protected Matrix ProjMatrix;
        protected Matrix CubeMatrix;

        protected RasterizerState RasterizeState;
        protected Buffer VertexShaderBuffer;
        protected Buffer PixelShaderBuffer;

        protected Texture2D RenderTex;
        protected ShaderResourceView RenderResource;
        protected RenderTargetView RenderView;

        protected Texture2D DepthTex;
        protected DepthStencilView DepthView;

        public Vec2 LastSize = new( 100, 100 );
        public Vec2 LastMid = new( 50, 50 );

        protected readonly DirectXDrawable Cube;

        public ModelRenderer( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            VertexShaderBuffer = new Buffer( Device, Utilities.SizeOf<VSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            PixelShaderBuffer = new Buffer( Device, Utilities.SizeOf<PSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            ViewMatrix = Matrix.LookAtLH( new Vector3( 0, 0, -Distance ), Position, Vector3.UnitY );
            RefreshRasterizeState();
            ResizeResources();

            // ======= CUBE ==========

            Cube = new( Device, Path.Combine( shaderPath, "Cube.fx" ), 3, false, false,
                new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "COLOR", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                } );

            var builder = new MeshBuilder( true, false );
            builder.AddBox( new( 0 ), 0.42f, 0.42f, 0.42f ); // 24 points total (6 faces * 4 corners)

            var colors = new List<Vector4>();
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0.75f, 0, 0, 1 ) );
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0, 0.75f, 0, 1 ) );
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0, 0, 0.75f, 1 ) );

            var data = FromMeshBuilder( builder, colors, false, false, false, out var cubeCount );
            Cube.SetVertexes( Device, data, cubeCount );
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
                SlopeScaledDepthBias = 0,
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

            RenderResource?.Dispose();
            RenderResource = new ShaderResourceView( Device, RenderTex );

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
            CameraPosition = Position - Distance * lookDirection;

            ViewMatrix = Matrix.LookAtLH( CameraPosition, Position, Vector3.UnitY );
            CubeMatrix = Matrix.LookAtLH( new Vector3( 0 ) - 1 * lookDirection, new Vector3( 0 ), Vector3.UnitY );
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

        public void Redraw() { NeedsRedraw = true; }

        public override void Draw() {
            BeforeDraw( out var oldState, out var oldRenderViews, out var oldDepthStencilView, out var oldDepthStencilState );

            var viewProj = Matrix.Multiply( ViewMatrix, ProjMatrix );

            var cubeProj = Matrix.Multiply( CubeMatrix, Matrix.PerspectiveFovLH( ( float )Math.PI / 4.0f, 1.0f, 0.1f, 100.0f ) );
            var world = LocalMatrix;

            viewProj.Transpose();
            cubeProj.Transpose();
            world.Transpose();

            var vsBuffer = new VSBufferStruct {
                ModelMatrix = world,
                ViewProjectionMatrix = viewProj,
                CubeMatrix = cubeProj,
                ProjectionMatrix = ProjMatrix,
                ViewMatrix = ViewMatrix,
                NormalMatrix = Matrix.Invert( LocalMatrix )
            };

            var psBuffer = new PSBufferStruct {
                ShowEdges = ShowEdges() ? 1 : 0,
            };

            Ctx.UpdateSubresource( ref vsBuffer, VertexShaderBuffer );
            Ctx.UpdateSubresource( ref psBuffer, PixelShaderBuffer );

            Ctx.OutputMerger.SetTargets( DepthView, RenderView );
            Ctx.ClearDepthStencilView( DepthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0 );
            Ctx.ClearRenderTargetView( RenderView, new Color4(
                Plugin.Configuration.RendererBackground.X,
                Plugin.Configuration.RendererBackground.Y,
                Plugin.Configuration.RendererBackground.Z,
                Plugin.Configuration.RendererBackground.W
            ) );

            Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, Width, Height, 0.0f, 1.0f ) );
            Ctx.Rasterizer.State = RasterizeState;

            OnDraw();
            Ctx.Flush();

            Ctx.Rasterizer.SetViewport( new Viewport( 0, 0, 80, 80, 0.0f, 1.0f ) );
            Cube.Draw( Ctx, VertexShaderBuffer, PixelShaderBuffer );
            Ctx.Flush();

            AfterDraw( oldState, oldRenderViews, oldDepthStencilView, oldDepthStencilState );
        }

        public virtual void DrawInline() {
            using var child = ImRaii.Child( "3DChild" );
            DrawImage();
        }

        protected virtual bool CanDrag() => true;

        protected void DrawImage() {
            if( NeedsRedraw ) {
                Draw();
                NeedsRedraw = false;
            }

            var cursor = ImGui.GetCursorScreenPos();
            var size = ImGui.GetContentRegionAvail();
            Resize( size );

            var pos = ImGui.GetCursorScreenPos();
            ImGui.ImageButton( Output, size, new Vec2( 0, 0 ), new Vec2( 1, 1 ), 0 );

            var topLeft = ImGui.GetItemRectMin();
            var bottomRight = ImGui.GetItemRectMax();
            LastSize = bottomRight - topLeft;
            LastMid = topLeft + ( LastSize / 2f );

            // ==== BUTTONS ========

            var topRight = pos + new Vec2( LastSize.X - 5, 5 );

            if( DrawButton( "Reset", new Vec2( 43, 25 ), topRight - new Vec2( 43, 0 ) ) ) {
                LastMousePos = default;
                Yaw = default;
                Pitch = default;
                Position = new( 0, 0, 0 );
                Distance = 5;
                UpdateViewMatrix();
            }

            // ================

            if( CanDrag() && ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                Drag( ImGui.GetMouseDragDelta(), true );
            }
            else if( CanDrag() && ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) Zoom( ImGui.GetIO().MouseWheel );
        }

        private static bool DrawButton( string text, Vec2 size, Vec2 pos ) {
            var drawList = ImGui.GetWindowDrawList();
            var hovered = UiUtils.MouseOver( pos, pos + size );
            drawList.AddRectFilled( pos, pos + size, ImGui.GetColorU32( hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button ), ImGui.GetStyle().FrameRounding );
            drawList.AddText( pos + ImGui.GetStyle().FramePadding, ImGui.GetColorU32( ImGuiCol.Text ), text );
            if( hovered && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) return true;
            return false;
        }

        public abstract void OnDispose();

        public override void Dispose() {
            RasterizeState?.Dispose();
            RenderTex?.Dispose();
            RenderResource?.Dispose();
            RenderView?.Dispose();

            DepthTex?.Dispose();
            DepthView?.Dispose();

            VertexShaderBuffer?.Dispose();
            PixelShaderBuffer?.Dispose();

            Cube?.Dispose();
            OnDispose();
        }

        public static Vector4[] FromMeshBuilder( MeshBuilder builder, List<Vector4> colors, bool useTangents, bool useBiTangents, bool useUv, out int indexCount ) {
            var mesh = builder.ToMesh();
            mesh.Normals = mesh.CalculateNormals();

            var data = new List<Vector4>();
            for( var idx = 0; idx < mesh.Indices.Count; idx++ ) {
                var pointIdx = mesh.Indices[idx];

                var position = mesh.Positions[pointIdx];
                data.Add( new( position.X, position.Y, position.Z, 1 ) );

                if( colors != null ) {
                    var color = colors[pointIdx];
                    data.Add( new( color.X, color.Y, color.Z, color.W ) );
                }

                if( useTangents ) {
                    var tangent = mesh.Tangents[pointIdx];
                    data.Add( new( tangent.X, tangent.Y, tangent.Z, 0 ) );
                }

                if( useBiTangents ) {
                    var biTangent = mesh.BiTangents[pointIdx];
                    data.Add( new( biTangent.X, biTangent.Y, biTangent.Z, 0 ) );
                }

                if( useUv ) {
                    var uv = mesh.TextureCoordinates[pointIdx];
                    data.Add( new( uv.X, uv.Y, uv.X, uv.Y ) );
                }

                var normal = mesh.Normals[pointIdx];
                data.Add( new( normal.X, normal.Y, normal.Z, 0 ) );
            }

            indexCount = mesh.Indices.Count;
            return data.ToArray();
        }
    }
}
