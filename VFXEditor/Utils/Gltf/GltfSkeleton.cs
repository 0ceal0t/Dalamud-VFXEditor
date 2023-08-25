using Dalamud.Logging;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.Utils.Gltf {
    public static class GltfSkeleton {
        public static List<SklbBone> ImportSkeleton( string localPath, List<SklbBone> currentBones ) {
            var newBones = new List<SklbBone>();
            var model = ModelRoot.Load( localPath );
            var nodes = model.LogicalNodes;

            var newNames = new Dictionary<string, SklbBone>();
            var currentNames = currentBones.Select( x => x.Name.Value ).ToList();

            var boneToNode = new Dictionary<SklbBone, Node>();
            var nodeToBone = new Dictionary<Node, SklbBone>();

            // Transformations
            var boneId = 0;
            foreach( var node in nodes ) {
                var name = node.Name;
                if( name.ToLower().Contains( "armature" ) || name.ToLower().Contains( "mesh" ) ) continue;

                var bone = new SklbBone( boneId++ );
                var transform = node.LocalTransform;
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                bone.Position.Value = new( pos.X, pos.Y, pos.Z, 1 );
                bone.Rotation.Quat = new( rot.X, rot.Y, rot.Z, rot.W );
                bone.Scale.Value = new( scl.X, scl.Y, scl.Z, 0 );
                bone.Name.Value = name;

                newNames[name] = bone;
                newBones.Add( bone );

                boneToNode[bone] = node;
                nodeToBone[node] = bone;
            }

            // Children
            foreach( var bone in newBones ) {
                var node = boneToNode[bone];
                foreach( var child in node.VisualChildren ) {
                    nodeToBone[child].Parent = bone;
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

        public static void ExportSkeleton( List<SklbBone> skeletonBones, string path ) {
            var scene = new SceneBuilder();

            var dummyMesh = GetDummyMesh();

            var bones = new List<NodeBuilder>();
            var roots = new List<NodeBuilder>();

            for( var i = 0; i < skeletonBones.Count; i++ ) {
                var bone = skeletonBones[i];
                var node = new NodeBuilder( bone.Name.Value );
                var pos = new Vector3( bone.Pos.X, bone.Pos.Y, bone.Pos.Z );
                var quat = bone.Rot;
                var rot = new Quaternion( quat.X, quat.Y, quat.Z, quat.W );
                var scl = new Vector3( bone.Scl.X, bone.Scl.Y, bone.Scl.Z );
                node.SetLocalTransform( new AffineTransform( scl, rot, pos ), false );
                bones.Add( node );
            }

            for( var i = 0; i < skeletonBones.Count; i++ ) {
                var bone = skeletonBones[i];
                var parentIdx = bone.Parent == null ? -1 : skeletonBones.IndexOf( bone.Parent );
                if( parentIdx != -1 ) {
                    bones[parentIdx].AddNode( bones[i] );
                }
                else {
                    roots.Add( bones[i] );
                }
            }

            scene.AddSkinnedMesh( dummyMesh, Matrix4x4.Identity, bones.ToArray() );
            var armature = new NodeBuilder( "Armature" );
            roots.ForEach( armature.AddNode );
            scene.AddNode( armature );

            var model = scene.ToGltf2();
            model.SaveGLTF( path );
            PluginLog.Log( $"Saved GLTF to: {path}" );
        }

        public static MeshBuilder<VertexPosition, VertexEmpty, VertexJoints4> GetDummyMesh() {
            var dummyMesh = new MeshBuilder<VertexPosition, VertexEmpty, VertexJoints4>( "DUMMY_MESH" );
            var material = new MaterialBuilder( "material" );

            var p1 = new VertexPosition() {
                Position = new( 0.000001f, 0, 0 )
            };
            var p2 = new VertexPosition() {
                Position = new( 0, 0.000001f, 0 )
            };
            var p3 = new VertexPosition() {
                Position = new( 0, 0, 0.000001f )
            };

            dummyMesh.UsePrimitive( material ).AddTriangle(
                (p1, new VertexEmpty(), new VertexJoints4( 0 )),
                (p2, new VertexEmpty(), new VertexJoints4( 0 )),
                (p3, new VertexEmpty(), new VertexJoints4( 0 ))
                );

            return dummyMesh;
        }
    }
}
