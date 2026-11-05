using FFXIVClientStructs.Havok.Animation.Animation;
using FFXIVClientStructs.Havok.Animation.Motion;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using FFXIVClientStructs.Havok.Common.Base.Types;
using SharpGLTF.Animations;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Formats.PapFormat.Motion;
using VfxEditor.Interop;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.Structs;

namespace VfxEditor.Utils.Gltf {
    public class AnimationKeys {
        public readonly Dictionary<float, Vector3> ScaleKeys = [];
        public readonly Dictionary<float, Quaternion> RotateKeys = [];
        public readonly Dictionary<float, Vector3> TranslationKeys = [];
    }

    public class GltfImportResult {
        public bool Success;
        public string Error;
        public readonly List<string> Bound = [];
        public readonly List<string> Skipped = [];
    }

    // There's something weird going on with the direct roots (n_hara, n_throw)

    public static unsafe class GltfAnimation {
        public static void ExportAnimation( hkaSkeleton* skeleton, List<string> animationNames, List<PapMotion> motions, bool skipUnanimated, string path ) {
            var scene = new SceneBuilder();

            var names = new List<string>();
            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                names.Add( skeleton->Bones[i].Name.String );
            }

            var dummyMesh = GltfSkeleton.GetDummyMesh();

            var bones = new List<NodeBuilder>();
            var roots = new List<NodeBuilder>();

            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                var node = new NodeBuilder( skeleton->Bones[i].Name.String );
                var pose = skeleton->ReferencePose[i];
                var pos = new Vector3( pose.Translation.X, pose.Translation.Y, pose.Translation.Z );
                var rot = new Quaternion( pose.Rotation.X, pose.Rotation.Y, pose.Rotation.Z, pose.Rotation.W );
                var scl = new Vector3( pose.Scale.X, pose.Scale.Y, pose.Scale.Z );
                node.SetLocalTransform( new AffineTransform( scl, rot, pos ), false );
                bones.Add( node );
            }

            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                var parentIdx = skeleton->ParentIndices[i];
                if( parentIdx != -1 ) {
                    bones[parentIdx].AddNode( bones[i] );
                }
                else {
                    roots.Add( bones[i] );
                }
            }

            scene.AddSkinnedMesh( dummyMesh, Matrix4x4.Identity, [.. bones] );
            var armature = new NodeBuilder( "Armature" );
            roots.ForEach( armature.AddNode );
            scene.AddNode( armature );

            var model = scene.ToGltf2();

            for( var animIdx = 0; animIdx < animationNames.Count; animIdx++ ) {
                var animationName = animationNames[animIdx];
                var motion = motions[animIdx];

                var nameToKeys = new Dictionary<string, AnimationKeys>();
                for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                    nameToKeys[skeleton->Bones[i].Name.String] = new();
                }

                var animation = model.UseAnimation( animationName );
                var totalFrames = motion.TotalFrames;
                for( var frame = 0; frame < totalFrames; frame++ ) {
                    ExportKeys( nameToKeys, names, motion, frame * ( 1 / 30f ) );
                }

                var unanimated = skipUnanimated ? motion.GetUnanimatedBones() : null;

                var nodes = model.LogicalNodes;
                foreach( var node in nodes ) {
                    if( node.Name == null || !nameToKeys.ContainsKey( node.Name ) ) continue;
                    if( skipUnanimated ) {
                        var idx = names.IndexOf( node.Name );
                        if( unanimated.Contains( idx ) ) {
                            Dalamud.Log( $"Skipping unanimated node {node.Name} in animation {animationName}" );
                            continue;
                        }
                    }

                    var keys = nameToKeys[node.Name];
                    animation.CreateRotationChannel( node, keys.RotateKeys, true );
                    animation.CreateScaleChannel( node, keys.ScaleKeys, true );
                    animation.CreateTranslationChannel( node, keys.TranslationKeys, true );
                }
            }

            model.SaveGLTF( path );
            Dalamud.Log( $"Saved GLTF to: {path}" );
        }

        private static void ExportKeys( Dictionary<string, AnimationKeys> nameToKeys, List<string> names, PapMotion motion, float time ) {
            var resetTime = motion.AnimationControl->LocalTime;
            motion.AnimationControl->LocalTime = time;

            var skeleton = motion.AnimatedSkeleton;

            if( motion.HasMismatchedSkeleton() ) {
                motion.AnimationControl->LocalTime = resetTime;
                return;
            }

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
            motion.AnimationControl->LocalTime = resetTime;
        }

        // Lord have mercy
        // https://github.com/0ceal0t/BlenderAssist/blob/main/BlenderAssist/pack_anim.cpp#L13
        // https://github.com/Etheirys/XAT/blob/main/Havok/Source/Conversion/FbxToHavok.cpp

        public static List<string> GetSkipBones( PapMotion motion, List<string> boneNames, bool skipUnanimated, List<string> excludeBones ) {
            var skipBones = new List<string>( excludeBones );
            if( skipUnanimated ) {
                skipBones.AddRange( motion.GetUnanimatedBones().Select( x => boneNames[x] ) );
            }
            return skipBones;
        }

        /// Determine which bones become transform tracks for one specific glTF animation.
        public static void GetImportTracks(
            SharpGLTF.Schema2.Animation animation,
            List<string> boneNames,
            ICollection<string> skipBones,
            List<string> tracks,
            List<string> skipped
        ) {
            var seen = new HashSet<string>();
            foreach( var channel in animation.Channels ) {
                var name = channel.TargetNode?.Name;
                if( string.IsNullOrEmpty( name ) || !seen.Add( name ) ) continue;
                if( !boneNames.Contains( name ) || skipBones.Contains( name ) ) {
                    skipped.Add( name );
                    continue;
                }
                tracks.Add( name );
            }

            // Keep tracks in skeleton order
            tracks.Sort( ( a, b ) => boneNames.IndexOf( a ).CompareTo( boneNames.IndexOf( b ) ) );
        }

        private static T* AllocZeroed<T>( HashSet<nint> handles ) where T : unmanaged {
            // Use AllocHGlobal (not NativeMemory) since handles are released with FreeHGlobal
            var size = Marshal.SizeOf<T>();
            var ptr = Marshal.AllocHGlobal( size );
            NativeMemory.Clear( ( void* )ptr, ( nuint )size );
            handles.Add( ptr );
            return ( T* )ptr;
        }

        public static GltfImportResult ImportAnimation(
            HashSet<nint> handles,
            hkaSkeleton* skeleton,
            PapMotion motion,
            int havokIndex,
            int gltfAnimationIndex,
            bool compress,
            bool skipUnanimated,
            List<string> excludeBones,
            string path
         ) {
            var result = new GltfImportResult();

            ModelRoot model;
            try {
                model = ModelRoot.Load( path );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Failed to load {path}" );
                result.Error = $"Could not load glTF: {e.Message}";
                return result;
            }

            var animations = model.LogicalAnimations;
            if( animations.Count == 0 ) {
                result.Error = "File has no animations";
                return result;
            }
            if( gltfAnimationIndex < 0 || gltfAnimationIndex >= animations.Count ) {
                result.Error = $"Invalid animation index {gltfAnimationIndex}";
                return result;
            }

            var boneNames = new List<string>();
            var boneNameToIdx = new Dictionary<string, int>();
            var refPoses = new Dictionary<string, hkQsTransformf>();
            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                var name = skeleton->Bones[i].Name.String;
                boneNames.Add( name );
                boneNameToIdx[name] = i;
                refPoses[name] = skeleton->ReferencePose[i];
            }

            var animation = animations[gltfAnimationIndex];
            var skipBones = GetSkipBones( motion, boneNames, skipUnanimated, excludeBones );
            var tracks = result.Bound;
            GetImportTracks( animation, boneNames, skipBones, tracks, result.Skipped );
            foreach( var name in result.Skipped ) Dalamud.Log( $"Skipped gLTF node: {name}" );

            if( tracks.Count == 0 ) {
                result.Error = $"Animation \"{animation.Name}\" does not animate any bones in the skeleton";
                return result;
            }

            var posSamplers = new Dictionary<string, ICurveSampler<Vector3>>();
            var rotSamplers = new Dictionary<string, ICurveSampler<Quaternion>>();
            var sclSamplers = new Dictionary<string, ICurveSampler<Vector3>>();

            foreach( var channel in animation.Channels ) {
                var name = channel.TargetNode?.Name;
                if( string.IsNullOrEmpty( name ) ) continue;
                var pos = channel.GetTranslationSampler()?.CreateCurveSampler();
                var rot = channel.GetRotationSampler()?.CreateCurveSampler();
                var scl = channel.GetScaleSampler()?.CreateCurveSampler();

                if( pos != null ) posSamplers[name] = pos;
                if( rot != null ) rotSamplers[name] = rot;
                if( scl != null ) sclSamplers[name] = scl;
            }

            // Uncompressed animations assume frames are evenly spread across the duration,
            // so the duration has to be an exact multiple of the frame time.
            // Always use at least 2 frames so the duration is never 0 (the compressor and time-to-frame divide by it)
            var numberOfFrames = Math.Max( 2, ( int )MathF.Round( animation.Duration * 30f ) + 1 );
            var duration = ( numberOfFrames - 1 ) / 30f;

            var transforms = new List<hkQsTransformf>( numberOfFrames * tracks.Count ); // frame-major, track-minor
            var prevRotations = new Quaternion?[tracks.Count];

            for( var frame = 0; frame < numberOfFrames; frame++ ) {
                var time = Math.Min( frame / 30f, animation.Duration );

                for( var trackIdx = 0; trackIdx < tracks.Count; trackIdx++ ) {
                    var track = tracks[trackIdx];
                    var refPose = refPoses[track];
                    var pos = refPose.Translation;
                    var rot = refPose.Rotation;
                    var scl = refPose.Scale;

                    // if we can find the samplers, use that
                    // otherwise use the ref pose

                    if( posSamplers.TryGetValue( track, out var posSampler ) ) {
                        var _pos = posSampler.GetPoint( time );
                        pos = new() { X = _pos.X, Y = _pos.Y, Z = _pos.Z, W = 1 };
                    }

                    var q = new Quaternion( rot.X, rot.Y, rot.Z, rot.W );
                    if( rotSamplers.TryGetValue( track, out var rotSampler ) ) {
                        var _rot = rotSampler.GetPoint( time );
                        q = new(
                            GltfSkeleton.Cleanup( _rot.X ),
                            GltfSkeleton.Cleanup( _rot.Y ),
                            GltfSkeleton.Cleanup( _rot.Z ),
                            GltfSkeleton.Cleanup( _rot.W )
                        );
                    }

                    q = q.LengthSquared() > 1e-12f ? Quaternion.Normalize( q ) : Quaternion.Identity;

                    if( prevRotations[trackIdx] is Quaternion prev && Quaternion.Dot( q, prev ) < 0 ) {
                        q = Quaternion.Negate( q );
                    }
                    prevRotations[trackIdx] = q;
                    rot = new() { X = q.X, Y = q.Y, Z = q.Z, W = q.W };

                    if( sclSamplers.TryGetValue( track, out var sclSampler ) ) {
                        var _scl = sclSampler.GetPoint( time );
                        scl = new() { X = _scl.X, Y = _scl.Y, Z = _scl.Z, W = 1 };
                    }

                    transforms.Add( new hkQsTransformf() {
                        Translation = pos,
                        Rotation = rot,
                        Scale = scl
                    } );
                }
            }

            var currentBinding = motion.AnimationControl->Binding;
            var currentAnim = currentBinding.ptr->Animation;

            var anim = AllocZeroed<HkaInterleavedUncompressedAnimation>( handles );
            ( ( HkBaseObject* )anim )->vfptr = ( HkBaseObject.HkBaseObjectVtbl* )ResourceLoader.HavokInterleavedAnimationVtbl;
            if( currentAnim.ptr != null ) anim->Animation.MemSizeAndRefCount = currentAnim.ptr->MemSizeAndRefCount;

            var binding = AllocZeroed<hkaAnimationBinding>( handles );
            ( ( HkBaseObject* )binding )->vfptr = ( ( HkBaseObject* )currentBinding.ptr )->vfptr;
            binding->MemSizeAndRefCount = currentBinding.ptr->MemSizeAndRefCount;

            // Set up binding
            binding->OriginalSkeletonName = currentBinding.ptr->OriginalSkeletonName;
            binding->BlendHint = currentBinding.ptr->BlendHint;

            var flags = currentBinding.ptr->TransformTrackToBoneIndices.Flags;

            binding->PartitionIndices = HavokData.CreateArray( handles, currentBinding.ptr->PartitionIndices, [] );
            binding->FloatTrackToFloatSlotIndices = HavokData.CreateArray( handles, currentBinding.ptr->FloatTrackToFloatSlotIndices, [] );
            binding->TransformTrackToBoneIndices = HavokData.CreateArray(
                handles, currentBinding.ptr->TransformTrackToBoneIndices, [.. tracks.Select( x => ( short )boneNameToIdx[x] )] );

            // Set up animation
            anim->Animation.Type = hkaAnimation.AnimationType.InterleavedAnimation;
            anim->Animation.Duration = duration;
            anim->Animation.NumberOfTransformTracks = tracks.Count;
            anim->Animation.NumberOfFloatTracks = 0;
            anim->Animation.ExtractedMotion = new hkRefPtr<hkaAnimatedReferenceFrame> { ptr = null };
            anim->Animation.AnnotationTracks = HavokData.CreateArray( handles, ( uint )flags, new List<hkaAnnotationTrack>(), Marshal.SizeOf<hkaAnnotationTrack>() );
            anim->Floats = HavokData.CreateArray( handles, ( uint )flags, new List<float>(), sizeof( float ) );
            anim->Transforms = HavokData.CreateArray( handles, ( uint )flags, transforms, Marshal.SizeOf<hkQsTransformf>() );

            var finalAnim = ( hkaAnimation* )anim;

            if( compress ) {
                Dalamud.Log( "Compressing animation..." );

                var spline = AllocZeroed<HkaSplineCompressedAnimation>( handles );
                Plugin.ResourceLoader.HavokSplineCtor( spline, anim );
                finalAnim = ( hkaAnimation* )spline;
            }

            var animPtr = new hkRefPtr<hkaAnimation>() { ptr = finalAnim };
            var bindingPtr = new hkRefPtr<hkaAnimationBinding>() { ptr = binding };
            binding->Animation = animPtr;

            var container = motion.File.MotionData.AnimationContainer;
            var anims = HavokData.ToList( container->Animations );
            var bindings = HavokData.ToList( container->Bindings );
            anims[havokIndex] = animPtr;
            bindings[havokIndex] = bindingPtr;

            container->Animations = HavokData.CreateArray( handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
            container->Bindings = HavokData.CreateArray( handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );

            Dalamud.Log( $"Imported \"{animation.Name}\": bound {tracks.Count} of {tracks.Count + result.Skipped.Count} animated nodes, {numberOfFrames} frames, {duration}s" );
            result.Success = true;
            return result;
        }
    }
}