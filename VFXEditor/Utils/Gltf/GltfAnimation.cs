using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.PapFormat.Skeleton;

namespace VfxEditor.Utils.Gltf {
    public class AnimationKeys {
        public readonly Dictionary<float, Vector3> ScaleKeys = new();
        public readonly Dictionary<float, Quaternion> RotateKeys = new();
        public readonly Dictionary<float, Vector3> TranslationKeys = new();
    }

    // There's something weird going on with the direct roots (n_hara, n_throw)

    public static unsafe class GltfAnimation {
        public static void ExportAnimation( hkaSkeleton* skeleton, string animationName, PapAnimatedSkeleton animatedSkeleton, string path ) {
            var scene = new SceneBuilder();

            var nameToKeys = new Dictionary<string, AnimationKeys>();
            var names = new List<string>();
            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                var name = skeleton->Bones[i].Name.String;
                names.Add( name );
                nameToKeys[name] = new();
            }

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

            var bones = new List<NodeBuilder>();
            var joints = new List<NodeBuilder>();
            var roots = new List<NodeBuilder>();

            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                var node = new NodeBuilder( skeleton->Bones[i].Name.String );
                var pose = skeleton->ReferencePose[i];
                var pos = new Vector3( pose.Translation.X, pose.Translation.Y, pose.Translation.Z );
                var rot = new Quaternion( pose.Rotation.X, pose.Rotation.Y, pose.Rotation.Z, pose.Rotation.W );
                var scl = new Vector3( pose.Scale.X, pose.Scale.Y, pose.Scale.Z );
                node.SetLocalTransform( new AffineTransform( scl, rot, pos ), false );
                bones.Add( node );

                // ============
                var parentIdx = skeleton->ParentIndices[i];
                if( parentIdx != -1 ) {
                    bones[parentIdx].AddNode( bones[i] );
                    joints.Add( bones[i] );
                }
                else {
                    roots.Add( bones[i] );
                }
            }

            scene.AddSkinnedMesh( dummyMesh, Matrix4x4.Identity, bones.ToArray() );
            var r = new NodeBuilder( "Armature" );
            roots.ForEach( x => r.AddNode( x ) );
            scene.AddNode( r );

            var model = scene.ToGltf2();

            var animation = model.UseAnimation( animationName );
            for( var time = 0f; time <= animatedSkeleton.Duration; time += 1 / 30f ) {
                ExportKeys( nameToKeys, names, animatedSkeleton, time );
            }

            var nodes = model.LogicalNodes;
            foreach( var node in nodes ) {
                if( node.Name == null || !nameToKeys.ContainsKey( node.Name ) ) continue;

                var keys = nameToKeys[node.Name];
                animation.CreateRotationChannel( node, keys.RotateKeys, true );
                animation.CreateScaleChannel( node, keys.ScaleKeys, true );
                animation.CreateTranslationChannel( node, keys.TranslationKeys, true );
            }

            model.SaveGLTF( path );
            PluginLog.Log( $"Saved GLTF to: {path}" );
        }

        private static void ExportKeys( Dictionary<string, AnimationKeys> nameToKeys, List<string> names, PapAnimatedSkeleton animatedSkeleton, float time ) {
            var resetTime = animatedSkeleton.Animation->LocalTime;
            animatedSkeleton.Animation->LocalTime = time;

            var skeleton = animatedSkeleton.Skeleton;

            var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( skeleton->Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
            var floats = ( float* )Marshal.AllocHGlobal( skeleton->Skeleton->FloatSlots.Length * sizeof( float ) );
            skeleton->sampleAndCombineAnimations( transforms, floats );

            for( var i = 0; i < names.Count; i++ ) {
                var name = names[i];
                var key = nameToKeys[name];

                var transform = transforms[i];
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                key.TranslationKeys[time] = new( pos.X, pos.Y, pos.Z );
                key.RotateKeys[time] = new( rot.X, rot.Y, rot.Z, rot.W );
                key.ScaleKeys[time] = new( scl.X, scl.Y, scl.Z );
            }

            // Reset
            Marshal.FreeHGlobal( ( nint )transforms );
            Marshal.FreeHGlobal( ( nint )floats );
            animatedSkeleton.Animation->LocalTime = resetTime;
        }

        public static void ImportAnimation( string localPath ) {
            var model = ModelRoot.Load( localPath );
            var nodes = model.LogicalNodes;
            var animations = model.LogicalAnimations;

            // nodes have item.IsTransformAnimated, check that before animating
            // animation name like "n_root|HavokAnimation|Base Layer", blender convention
            // animation tragetNodePath = translation|rotation|scale

            foreach( var item in nodes ) {
                //PluginLog.Log( $"{item.IsTransformAnimated} {item.Name}" );
                var local = item.LocalTransform;
                //PluginLog.Log( "---------------" );
            }

            foreach( var item in animations ) {
                PluginLog.Log( $"{item.Name}" );
                foreach( var c in item.Channels ) {
                    var t = c.GetTranslationSampler();
                    var r = c.GetRotationSampler();
                    var s = c.GetScaleSampler();

                    // t.CreateCurveSampler(); // can use to query at a given time

                    PluginLog.Log( $"{c.TargetNodePath} {c.TargetNode.Name} {t?.InterpolationMode} {r?.InterpolationMode} {s?.InterpolationMode}" );
                }
                PluginLog.Log( "================" );
            }
        }
    }
}
