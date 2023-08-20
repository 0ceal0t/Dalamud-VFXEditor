using Dalamud.Logging;
using OtterGui;
using SharpGLTF.Scenes;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.Utils.Gltf {
    public static class GltfSkeleton {
        public static List<SklbBone> ImportSkeleton( string localPath, List<SklbBone> currentBones ) {
            var newBones = new List<SklbBone>();
            var model = SharpGLTF.Schema2.ModelRoot.Load( localPath );
            var nodes = model.LogicalNodes;

            var newNames = new Dictionary<string, SklbBone>();
            var currentNames = currentBones.Select( x => x.Name.Value ).ToList();

            // Transformations
            var id = 0;
            foreach( var node in nodes ) {
                var bone = new SklbBone( id++ );
                var transform = node.LocalTransform;
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                bone.Position.Value = new( pos.X, pos.Y, pos.Z, 1 );
                bone.Rotation.Value = new( rot.X, rot.Y, rot.Z, rot.W );
                bone.Scale.Value = new( scl.X, scl.Y, scl.Z, 0 );
                bone.Name.Value = node.Name;

                newNames[bone.Name.Value] = bone;
                newBones.Add( bone );
            }

            // Children
            for( var i = 0; i < newBones.Count; i++ ) {
                var bone = newBones[i];
                var node = nodes[i];
                foreach( var nodeChild in node.VisualChildren ) {
                    var childIdx = nodes.IndexOf( nodeChild );
                    var boneChild = newBones[childIdx];
                    boneChild.Parent = bone;
                }
            }

            // Try to match imported bone indexes to the current ones as much as possible
            // Otherwise the indexes can be mismatched and break the preview, so this just makes it nicer
            var unusedBones = new List<SklbBone>();
            unusedBones.AddRange( newBones );
            var finalBones = new List<SklbBone>();

            for( var i = 0; i < newBones.Count; i++ ) {
                if( i >= currentNames.Count ) {
                    finalBones.Add( null );
                    continue;
                }

                var bone = newNames.TryGetValue( currentNames[i], out var newBone ) ? newBone : null;
                if( bone != null ) unusedBones.Remove( bone ); // Found a bone with the same name, use the same index
                finalBones.Add( bone );
            }

            // Fill in the remaining slots
            for( var i = 0; i < finalBones.Count; i++ ) {
                if( finalBones[i] != null ) continue;
                var bone = unusedBones[0];
                finalBones[i] = bone;
                unusedBones.Remove( bone );
            }

            return finalBones;
        }

        public static void ExportSkeleton( List<SklbBone> bones, string path ) {
            var scene = new SceneBuilder();

            var rootBones = bones.Where( x => x.Parent == null ).ToList();
            foreach( var bone in rootBones ) {
                var node = CreateNode( bone, bones, null );
                scene.AddNode( node );
            }

            scene.ToGltf2().SaveGLTF( path );
            PluginLog.Log( $"Saved GLTF to: {path}" );
        }

        private static NodeBuilder CreateNode( SklbBone bone, List<SklbBone> bones, NodeBuilder parent ) {
            var children = bones.Where( x => x.Parent == bone ).ToList();
            var name = bone.Name.Value;
            var pos = bone.Position.Value;
            var rot = bone.Rotation.Value;
            var scl = bone.Scale.Value;

            var node = parent == null ? new NodeBuilder( name ) : parent.CreateNode( name );
            node.WithLocalTranslation( new Vector3( pos.X, pos.Y, pos.Z ) );
            node.WithLocalRotation( new Quaternion( rot.X, rot.Y, rot.Z, rot.W ) );
            node.WithLocalScale( new Vector3( scl.X, scl.Y, scl.Z ) );

            children.ForEach( x => CreateNode( x, bones, node ) );
            return node;
        }
    }
}
