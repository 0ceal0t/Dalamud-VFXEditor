using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using AVFXLib.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace VFXEditor.Data.DirectX {
    public class UVPreview : ModelView {
        public UI.VFX.Particle.UVSet.UVAnimation _CurrentUV = null;

        // ======= BASE MODEL =======
        public static int MODEL_SPAN = 3; // position, uv, normal
        public int NumVerts;
        public Buffer Vertices;
        CompilationResult vertexShaderByteCode;
        CompilationResult pixelShaderByteCode;
        PixelShader PShader;
        VertexShader VShader;
        ShaderSignature Signature;
        InputLayout Layout;

        // ======== TEXTURE ===========
        Texture2D Texture;
        ShaderResourceView ShaderTexture;
        SamplerState Sampler;
        Buffer AnimDataBuffer;
        public Matrix AnimData = new Matrix(0);

        public UVPreview( DirectXManager manager ) : base( manager ) {
            NumVerts = 0;
            Vertices = null;

            // ======= BASE MODEL =========
            vertexShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "UVPreview_VS.fx" ), "VS", "vs_4_0" );
            pixelShaderByteCode = ShaderBytecode.CompileFromFile( Path.Combine( Manager.ShaderPath, "UVPreview_PS.fx" ), "PS", "ps_4_0" );
            VShader = new VertexShader( Device, vertexShaderByteCode );
            PShader = new PixelShader( Device, pixelShaderByteCode );
            Signature = ShaderSignature.GetInputSignature( vertexShaderByteCode );
            Layout = new InputLayout( Device, Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("UV", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
            } );

            // ========== TEXTURE ==========
            var samplerStateDescription = new SamplerStateDescription {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear
            };
            Sampler = new SamplerState( Device, samplerStateDescription );
            AnimDataBuffer = new Buffer( Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0 );
        }

        public void LoadModel( AVFXModel model) {
            if(model == null ) {
                LoadModel( new List<Index>(), new List<Vertex>() );
                return;
            }
            LoadModel( model.Indexes, model.Vertices);
        }

        public void LoadModel( List<Index> _Indexes, List<Vertex> _Vertices) {
            if( _Indexes.Count == 0 ) {
                NumVerts = 0;
                Vertices?.Dispose();
            }
            else {
                Vector4[] data = new Vector4[_Indexes.Count * 3 * MODEL_SPAN]; // 3 vertices per face
                for( int index = 0; index < _Indexes.Count; index++ ) { // each face
                    int[] _indexes = new int[] { _Indexes[index].I1, _Indexes[index].I2, _Indexes[index].I3 };
                    for( int j = 0; j < _indexes.Length; j++ ) { // push all 3 vertices per face
                        var idx = GetIdx( index, j, MODEL_SPAN, 3 );
                        var v = _Vertices[_indexes[j]];
                        data[idx + 0] = new Vector4( v.Position[0], v.Position[1], v.Position[2], 1.0f );
                        data[idx + 1] = ( new Vector4( v.UV1[0], v.UV1[1], v.UV2[2], v.UV2[3] ) + 0.5f );
                        data[idx + 2] = new Vector4( v.Normal[0], v.Normal[1], v.Normal[2], 1.0f );
                    }
                }
                Vertices?.Dispose();
                Vertices = Buffer.Create( Device, BindFlags.VertexBuffer, data );
                NumVerts = _Indexes.Count * 3;
            }

            UpdateDraw();
        }

        public void LoadTexture(Bitmap bitmap ) {
            Texture?.Dispose();
            ShaderTexture?.Dispose();

            var data = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            Texture = new Texture2D( Device, new Texture2DDescription() {
                Width = bitmap.Width,
                Height = bitmap.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Immutable,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription( 1, 0 ),
            }, new DataRectangle( data.Scan0, data.Stride ) );
            bitmap.UnlockBits( data );

            ShaderTexture = new ShaderResourceView( Device, Texture );

            if( FirstModel ) {
                Draw();
            }
        }


        public override void OnDraw() {
            // ====== DRAW BASE =========
            Ctx.PixelShader.Set( PShader );
            Ctx.VertexShader.Set( VShader );
            Ctx.InputAssembler.InputLayout = Layout;
            Ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Ctx.VertexShader.SetConstantBuffer( 0, WorldBuffer );

            // ====== TEXTURE ============
            Ctx.UpdateSubresource( ref AnimData, AnimDataBuffer );
            Ctx.PixelShader.SetConstantBuffer( 0, AnimDataBuffer );
            Ctx.PixelShader.SetShaderResource( 0, ShaderTexture );
            Ctx.PixelShader.SetSampler( 0, Sampler );

            if( NumVerts > 0 ) {
                Ctx.InputAssembler.SetVertexBuffers( 0, new VertexBufferBinding( Vertices, Utilities.SizeOf<Vector4>() * MODEL_SPAN, 0 ) ); // set vertex buffer
                Ctx.Draw( NumVerts, 0 );
            }
        }

        public override void OnDispose() {
            Texture?.Dispose();
            Sampler?.Dispose();
            ShaderTexture?.Dispose();
            AnimDataBuffer?.Dispose();

            Vertices?.Dispose();
            Layout?.Dispose();
            Signature?.Dispose();
            PShader?.Dispose();
            VShader?.Dispose();
            vertexShaderByteCode?.Dispose();
            pixelShaderByteCode?.Dispose();
        }
    }
}
