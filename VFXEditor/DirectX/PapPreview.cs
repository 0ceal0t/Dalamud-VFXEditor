using HelixToolkit.SharpDX.Core;
using SharpDX.Direct3D11;
using VfxEditor.PapFormat;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class PapPreview : AnimationPreview {
        public PapFile CurrentFile { get; private set; }
        public PapAnimation CurrentAnimation { get; private set; }

        public PapPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PapFile file, PapAnimation animation, BoneSkinnedMeshGeometry3D mesh ) {
            CurrentFile = file;
            CurrentAnimation = animation;
            LoadSkeleton( mesh );
        }

        public void LoadEmpty( PapFile file, PapAnimation animation ) {
            CurrentFile = file;
            CurrentAnimation = animation;
            NumVertices = 0;
            Vertices?.Dispose();
            UpdateDraw();
        }

        public void ClearFile() {
            CurrentFile = null;
        }

        public void ClearAnimation() {
            CurrentAnimation = null;
        }
    }
}
