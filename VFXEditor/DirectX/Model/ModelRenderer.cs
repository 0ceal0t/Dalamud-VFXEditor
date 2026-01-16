using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.DirectX.Drawable;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Model {
    [StructLayout( LayoutKind.Sequential )]
    public struct VSBufferStruct {
        public Matrix4x4 ModelMatrix;
        public Matrix4x4 ViewMatrix;
        public Matrix4x4 ProjectionMatrix;
        public Matrix4x4 ViewProjectionMatrix;
        public Matrix4x4 NormalMatrix;
        public Matrix4x4 CubeMatrix;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PSBufferStruct {
        public int ShowEdges;
        public Vector2 Size;
        public float _Pad0;
    }

    [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
    public struct LightData {
        public Vector3 Color;
        public float Radius;
        public Vector3 Position;
        public float Falloff;
    }

    public abstract class ModelRenderer<T> : Renderer where T : ModelInstance {
        protected RasterizerState RasterizeState;
        protected Buffer VertexShaderBuffer;
        protected Buffer PixelShaderBuffer;

        protected SamplerState Sampler;
        protected DepthStencilState StencilState;

        protected readonly D3dDrawable Cube;

        public ModelRenderer( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx ) {
            VertexShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<VSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            PixelShaderBuffer = new Buffer( Device, SharpDX.Utilities.SizeOf<PSBufferStruct>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
            RefreshRasterizeState();

            // ======= CUBE ==========

            Cube = new( 3, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "COLOR", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                ] );
            Cube.AddPass( device, PassType.Final, Path.Combine( shaderPath, "Cube.fx" ), ShaderPassFlags.Pixel );

            var builder = new MeshBuilder( true, false );
            builder.AddBox( new( 0 ), 0.42f, 0.42f, 0.42f ); // 24 points total (6 faces * 4 corners)

            var colors = new List<Vector4>();
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0.75f, 0, 0, 1 ) );
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0, 0.75f, 0, 1 ) );
            for( var i = 0; i < 8; i++ ) colors.Add( new( 0, 0, 0.75f, 1 ) );

            var data = FromMeshBuilder( builder, colors, false, false, false, out var cubeCount );
            Cube.SetVertexes( Device, data, cubeCount );

            // ======== DEPTH =========

            StencilState = new( Device, new() {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less
            } );

            Sampler = new( Device, new() {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = new( 0, 0, 0, 0 ),
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            } );
        }

        protected virtual bool ShowEdges() => Plugin.Configuration.ModelShowEdges && !Plugin.Configuration.ModelWireframe;

        protected void RefreshRasterizeState() {
            RasterizeState?.Dispose();
            RasterizeState = new RasterizerState( Device, new RasterizerStateDescription {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0,
            } );
        }

        protected abstract void RenderPasses( T instance );

        public void Render( T instance ) {
            instance.SetNeedsRedraw( false ); // Doesn't need an update

            BeforeRender( out var oldState, out var oldRenderViews, out var oldDepthStencilView, out var oldDepthStencilState );

            var viewProj = Matrix4x4.Multiply( instance.ViewMatrix, instance.ProjMatrix );

            var cubeProj = Matrix4x4.Multiply( instance.CubeMatrix, MatrixHelper.PerspectiveFovLH( ( float )Math.PI / 4.0f, 1.0f, 0.1f, 100.0f ) );
            var world = instance.LocalMatrix;

            viewProj = Matrix4x4.Transpose( viewProj );
            cubeProj = Matrix4x4.Transpose( cubeProj );
            world = Matrix4x4.Transpose( world );

            var vsBuffer = new VSBufferStruct {
                ModelMatrix = world,
                ViewProjectionMatrix = viewProj,
                CubeMatrix = cubeProj,
                ProjectionMatrix = instance.ProjMatrix,
                ViewMatrix = instance.ViewMatrix,
                NormalMatrix = Matrix4x4.Transpose( instance.LocalMatrix.Inverted() )
            };

            var psBuffer = new PSBufferStruct {
                ShowEdges = ShowEdges() ? 1 : 0,
                Size = new( instance.Width, instance.Height ),
            };

            Ctx.UpdateSubresource( ref vsBuffer, VertexShaderBuffer );
            Ctx.UpdateSubresource( ref psBuffer, PixelShaderBuffer );

            Ctx.ClearDepthStencilView( instance.StencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0 );
            Ctx.ClearRenderTargetView( instance.RenderTarget, new RawColor4(
                Plugin.Configuration.RendererBackground.X,
                Plugin.Configuration.RendererBackground.Y, 
                Plugin.Configuration.RendererBackground.Z,
                Plugin.Configuration.RendererBackground.W
            ) );

            Ctx.Rasterizer.SetViewport( 0, 0, instance.Width, instance.Height, 0.0f, 1.0f );
            Ctx.Rasterizer.State = RasterizeState;

            // ======= PASSES ======

            Ctx.OutputMerger.SetDepthStencilState( StencilState );
            Ctx.OutputMerger.SetTargets( instance.StencilView, instance.RenderTarget );

            RenderPasses( instance );
            Ctx.Flush();

            // ======= CUBE ===========

            Ctx.Rasterizer.SetViewport(  0, 0, 80, 80, 0.0f, 1.0f );

            Ctx.OutputMerger.SetDepthStencilState( StencilState );
            Ctx.OutputMerger.SetTargets( instance.StencilView, instance.RenderTarget );

            Cube.Draw( Ctx, PassType.Final, VertexShaderBuffer, PixelShaderBuffer );
            Ctx.Flush();

            AfterRender( oldState, oldRenderViews, oldDepthStencilView, oldDepthStencilState );
        }

        public void DrawTexture( int renderId, T instance, Action update ) {
            using var child = ImRaii.Child( "3DChild" );
            var needsUpdate = ( renderId != instance.CurrentRenderId || instance != LoadedInstance );
            if( needsUpdate ) {
                update();
            }

            if( instance.NeedsRedraw || instance.Resize( ImGui.GetContentRegionAvail() ) || needsUpdate ) {
                Render( instance ); // Needs a refresh
            }
            instance.DrawInstanceTexture();
        }

        public override void Dispose() {
            RasterizeState?.Dispose();
            StencilState?.Dispose();
            Sampler?.Dispose();

            VertexShaderBuffer?.Dispose();
            PixelShaderBuffer?.Dispose();

            Cube?.Dispose();
        }

        public static Vector4[] FromMeshBuilder( MeshBuilder builder, List<Vector4> colors, bool useTangents, bool useBiTangents, bool useUv, out int indexCount ) {
            var mesh = builder.ToMeshGeometry3D();
            mesh.UpdateNormals();

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
            return [.. data];
        }
    }
}
