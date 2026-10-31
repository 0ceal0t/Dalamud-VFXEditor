using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
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
            var currentNames = currentBones.WithIndex().ToDictionary( x => x.Value.Name.Value, x => x.Index );

            var boneToNode = new Dictionary<SklbBone, Node>();
            var nodeToBone = new Dictionary<Node, SklbBone>();

            // Transformations
            var boneId = 0;
            foreach( var node in nodes ) {
                if( node == null || node.Name == null ) continue;
                var name = node.Name;
                if( name.Contains( "armature", StringComparison.CurrentCultureIgnoreCase ) || name.Contains( "mesh", StringComparison.CurrentCultureIgnoreCase ) ) continue;

                var bone = new SklbBone( boneId++ );
                var transform = node.LocalTransform;
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                bone.Position.Value = new( pos.X, pos.Y, pos.Z, 1 );
                bone.Rotation.Quaternion = new(
                    Cleanup( rot.X ),
                    Cleanup( rot.Y ),
                    Cleanup( rot.Z ),
                    Cleanup( rot.W )
                );
                bone.Scale.Value = new( scl.X, scl.Y, scl.Z, 0 );
                bone.Name.Value = name;

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

            var finalBones = new List<SklbBone>();

            var root = newBones.Find( bone => bone.Name.Value == "n_root" );
            var rootNode = boneToNode[root];
            if( rootNode.VisualChildren.Count() != 0 )
            {
                Dictionary<int, List<SklbBone>> index = [];
                PopulateIndex( index, rootNode, nodeToBone, 0 );
                foreach( var i in index )
                {
                    foreach( var bone in i.Value )
                    {
                        finalBones.Add( bone );
                    }
                }
            } else
            {
                finalBones.Add( root );
            }

            return [.. finalBones.Where( x => x != null )];
        }

        private static void PopulateIndex( Dictionary<int, List<SklbBone>> index, Node node, Dictionary<Node, SklbBone> nodeToBone, int depth )
        {
            if( !index.ContainsKey( depth ) ) { index.Add( depth, [] ); }
            index[depth].Add( nodeToBone[node] );
            if( node.VisualChildren.Count() > 0 )
            {
                foreach( var child in node.VisualChildren )
                {
                    PopulateIndex( index, child, nodeToBone, depth + 1 );
                }
                
            }
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
                var rot = new Quaternion( ( float )quat.X, ( float )quat.Y, ( float )quat.Z, ( float )quat.W );
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

            var armature = new NodeBuilder( "Armature" );
            roots.ForEach( armature.AddNode );
            scene.AddSkinnedMesh( dummyMesh, Matrix4x4.Identity, [.. bones] );
            scene.AddNode( armature );

            var model = scene.ToGltf2();
            model.SaveGLTF( path );
            Dalamud.Log( $"Saved GLTF to: {path}" );
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

        public static float Cleanup( float value ) => Math.Abs( value ) < 0.0001f ? 0f : value;
    }
}
