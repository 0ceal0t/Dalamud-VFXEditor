using HelixToolkit.SharpDX.Core;
using SharpDX.Direct3D11;
using VfxEditor.PapFormat;
using VfxEditor.PapFormat.Motion;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class PapPreview : BonePreview {
        public PapFile CurrentFile { get; private set; }
        public PapMotion CurrentMotion { get; private set; }

        public PapPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PapFile file, PapMotion animation, BoneSkinnedMeshGeometry3D mesh ) {
            CurrentFile = file;
            CurrentMotion = animation;
            LoadSkeleton( mesh );
        }

        public void LoadEmpty( PapFile file, PapMotion animation ) {
            CurrentFile = file;
            CurrentMotion = animation;
            NumVertices = 0;
            Vertices?.Dispose();
            UpdateDraw();
        }

        public void ClearFile() {
            CurrentFile = null;
        }

        public void ClearAnimation() {
            CurrentMotion = null;
        }
    }
}
