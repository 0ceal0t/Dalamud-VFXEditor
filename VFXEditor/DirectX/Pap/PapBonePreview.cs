using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;
using SharpDX.Direct3D11;
using VfxEditor.Formats.PapFormat.Motion.Preview;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Pap {
    public class PapBonePreview : BonePreview {
        public PapBonePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PapMotionSkeleton animation, BoneSkinnedMeshGeometry3D mesh ) {
            CurrentRenderId = animation.RenderId;
            LoadSkeleton( mesh );
        }

        public void LoadEmpty( PapMotionSkeleton animation ) {
            CurrentRenderId = animation.RenderId;
            Model.ClearVertexes();
            UpdateDraw();
        }
    }
}
