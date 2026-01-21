using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using VfxEditor.EidFormat;
using VfxEditor.Interop.Havok.Ui;

namespace VfxEditor.Formats.EidFormat.Skeleton {
    public class EidSkeletonView : SkeletonView {
        private readonly EidFile File;

        public EidSkeletonView( EidFile file, string sourcePath ) : base( file, file.BoneNameInstance, sourcePath, "eid" ) {
            File = file;
        }

        protected override void DrawData() {
            if( File.BindPointsUpdated ) Plugin.DirectXManager.BoneNameRenderer.NeedsUpdate = true;
        }

        protected override void UpdateData() {
            File.BindPointsUpdated = false;
            if( Bones?.BoneList.Count == 0 ) return;

            var mesh = new MeshBuilder( true, false );
            var selected = new MeshBuilder( true, false );

            File.AddBindPoints( mesh, selected, Bones.BoneMatrixes );
            Plugin.DirectXManager.BoneNameRenderer.SetWireFrame(
                RenderId,
                Instance,
                mesh.ToMeshGeometry3D(),
                new MeshBuilder( true, false ).ToMeshGeometry3D(),
                selected.ToMeshGeometry3D()
            );
        }
    }
}
