using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;

namespace VfxEditor.DirectX.Drawable {
    public enum PassType {
        Depth,
        Draw
    }

    [Flags]
    public enum ShaderPassFlags {
        None,
        Geometry,
        Pixel,
    }

    public class D3dPass {
        public readonly ShaderPassFlags Flags;

        private bool GeoEnabled => Flags.HasFlag( ShaderPassFlags.Geometry );
        private bool PixelEnabled => Flags.HasFlag( ShaderPassFlags.Pixel );

        public readonly CompilationResult CompiledGeometryShader;
        public readonly GeometryShader GeometryShader;

        public readonly CompilationResult CompiledVertexShader;
        public readonly VertexShader VertexShader;

        public readonly CompilationResult CompiledPixelShader;
        public readonly PixelShader PixelShader;

        public readonly ShaderSignature Signature;
        public readonly InputLayout Layout;

        public D3dPass( Device device, string path, ShaderPassFlags flags, InputElement[] layout ) {
            Flags = flags;

            CompiledVertexShader = ShaderBytecode.CompileFromFile( path, "VS", "vs_4_0", include: DirectXManager.IncludeHandler );
            VertexShader = new VertexShader( device, CompiledVertexShader );

            Signature = ShaderSignature.GetInputSignature( CompiledVertexShader );
            Layout = new InputLayout( device, Signature, layout );

            if( GeoEnabled ) {
                CompiledGeometryShader = ShaderBytecode.CompileFromFile( path, "GS", "gs_4_0", include: DirectXManager.IncludeHandler );
                GeometryShader = new GeometryShader( device, CompiledGeometryShader );
            }

            if( PixelEnabled ) {
                CompiledPixelShader = ShaderBytecode.CompileFromFile( path, "PS", "ps_4_0", include: DirectXManager.IncludeHandler );
                PixelShader = new PixelShader( device, CompiledPixelShader );
            }
        }

        public virtual void Setup( DeviceContext ctx ) {
            ctx.PixelShader.Set( PixelShader );
            ctx.GeometryShader.Set( GeometryShader );
            ctx.VertexShader.Set( VertexShader );
            ctx.InputAssembler.InputLayout = Layout;
            ctx.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        public void Dispose() {
            Layout?.Dispose();
            Signature?.Dispose();
            PixelShader?.Dispose();
            VertexShader?.Dispose();
            GeometryShader?.Dispose();
            CompiledPixelShader?.Dispose();
            CompiledVertexShader?.Dispose();
            CompiledGeometryShader?.Dispose();
        }
    }
}
