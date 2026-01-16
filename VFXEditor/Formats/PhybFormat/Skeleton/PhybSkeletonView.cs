using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;
using System.Linq;
using VfxEditor.Interop.Havok.Ui;

namespace VfxEditor.PhybFormat.Skeleton {
    public class PhybSkeletonView : SkeletonView {
        private readonly PhybFile File;

        public PhybSkeletonView( PhybFile file, string sourcePath ) : base( file, file.Instance, sourcePath, "phyb" ) {
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
            Plugin.DirectXManager.BoneNameRenderer.SetWireFrame( RenderId, Instance, meshes.Collision.ToMeshGeometry3D(), meshes.Simulation.ToMeshGeometry3D(), meshes.Spring.ToMeshGeometry3D() );

            var boneList = Bones.BoneList.Select( x => x.Name ).ToList();
            if( File.Extended != null ) {
                foreach( var bone in File
                    .Extended
                    .Table
                    .Alpha
                    .SelectMany( x => x.Beta )
                    .SelectMany( x => x.Gamma )
                    .Select( x => x.Bone )
                ) bone.Guess( boneList );
            }
        }
    }
}
