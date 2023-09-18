using HelixToolkit.SharpDX.Core;
using VfxEditor.Interop.Havok.Ui;

namespace VfxEditor.PhybFormat.Skeleton {
    public class PhybSkeletonView : SkeletonView {
        private readonly PhybFile File;

        public PhybSkeletonView( PhybFile file, string sourcePath ) : base( file, Plugin.DirectXManager.PhybPreview, sourcePath, "phyb" ) {
            File = file;
        }

        protected override void DrawData() {
            if( File.PhysicsUpdated ) UpdateData();
        }

        protected override void UpdateData() {
            File.PhysicsUpdated = false;
            if( Bones?.BoneList.Count == 0 ) return;

            MeshBuilders meshes = new() {
                Collision = new MeshBuilder( true, false ),
                Simulation = new MeshBuilder( true, false ),
                Spring = new MeshBuilder( true, false )
            };

            File.AddPhysicsObjects( meshes, Bones.BoneMatrixes );
            Preview.LoadWireframe( meshes.Collision.ToMesh(), meshes.Simulation.ToMesh(), meshes.Spring.ToMesh() );
        }
    }
}
