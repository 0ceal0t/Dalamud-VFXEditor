using HelixToolkit.SharpDX.Core;
using SharpDX.Direct3D11;
using VfxEditor.PapFormat.Motion;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX.Pap {
    public class PapBonePreview : BonePreview {
        public PapBonePreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        public void LoadSkeleton( PapMotion animation, BoneSkinnedMeshGeometry3D mesh ) {
            CurrentRenderId = animation.RenderId;
            LoadSkeleton( mesh );
        }

        public void LoadEmpty( PapMotion animation ) {
            CurrentRenderId = animation.RenderId;
            Model.ClearVertexes();
            UpdateDraw();
        }
    }
}
