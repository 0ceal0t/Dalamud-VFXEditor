using FFXIVClientStructs.Havok.Animation.Animation;
using FFXIVClientStructs.Havok.Animation.Playback;
using FFXIVClientStructs.Havok.Animation.Playback.Control;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.Formats.PapFormat.Motion.Preview;
using VfxEditor.Interop.Havok;
using VfxEditor.Parsing;

namespace VfxEditor.PapFormat.Motion {
    // https://github.com/soulsmods/DSMapStudio/blob/360245a095eb5db9dc821a213bc41b2b3ff3db0d/HKX2/Autogen/hkaInterleavedUncompressedAnimation.cs#L7
    // https://github.com/soulsmods/DSMapStudio/blob/360245a095eb5db9dc821a213bc41b2b3ff3db0d/HKX2/Autogen/hkaPredictiveCompressedAnimation.cs
    // https://github.com/soulsmods/DSMapStudio/blob/360245a095eb5db9dc821a213bc41b2b3ff3db0d/HKX2/Autogen/hkaQuantizedAnimation.cs
    // https://github.com/soulsmods/DSMapStudio/blob/360245a095eb5db9dc821a213bc41b2b3ff3db0d/HKX2/Autogen/hkaSplineCompressedAnimation.cs
    // https://github.com/soulsmods/DSMapStudio/blob/360245a095eb5db9dc821a213bc41b2b3ff3db0d/HKX2/Autogen/hkaInterleavedUncompressedAnimation.cs
    // https://github.com/0ceal0t/BlenderAssist/blob/main/BlenderAssist/pack_anim.cpp#L141

    public enum BlendHintTypes : int {
        Normal = 0x0,
        AdditiveDeprecated = 0x1,
        Additive = 0x2,
    }

    public unsafe class PapMotion {
        public readonly PapFile File;
        public readonly hkaAnimatedSkeleton* AnimatedSkeleton;
        public readonly hkaAnimationControl* AnimationControl;
        public hkaAnimationBinding* Binding => AnimationControl->Binding.ptr;
        public hkaAnimation* Animation => Binding->Animation.ptr;
        public hkaSkeleton* Skeleton => AnimatedSkeleton->Skeleton;

        public float Time => AnimationControl->LocalTime;
        public float Duration => Animation->Duration;
        public int TotalFrames => ( int )( Duration * 30f );

        private readonly ParsedString OriginalSkeletonName = new( "Original Skeleton Name" );
        private readonly ParsedEnum<BlendHintTypes> BlendHint = new( "Blend Hint" );
        private readonly PapMotionPreview Preview;

        public PapMotion( PapFile file, HavokData bones, hkaAnimationBinding* binding ) {
            File = file;
            AnimatedSkeleton = ( hkaAnimatedSkeleton* )Marshal.AllocHGlobal( Marshal.SizeOf<hkaAnimatedSkeleton>() );
            AnimationControl = ( hkaAnimationControl* )Marshal.AllocHGlobal( Marshal.SizeOf<hkaAnimationControl>() );

            AnimationControl->Ctor1( binding );
            AnimatedSkeleton->Ctor1( bones.AnimationContainer->Skeletons[0].ptr );
            AnimatedSkeleton->addAnimationControl( AnimationControl );

            OriginalSkeletonName.Value = Binding->OriginalSkeletonName.String;
            BlendHint.Value = ( BlendHintTypes )Binding->BlendHint.Value;

            Preview = file.IsMaterial ? new PapMotionMaterial( file, this ) : new PapMotionSkeleton( file, this );
        }

        public void DrawPreview( int idx ) => Preview.Draw( idx );

        public void DrawHavok() {
            ImGui.TextDisabled( $"{Animation->Type}" );
            OriginalSkeletonName.Draw();
            BlendHint.Draw();
        }

        public void UpdateHavok( HashSet<nint> handles ) {
            var nameHandle = Marshal.StringToHGlobalAnsi( OriginalSkeletonName.Value );
            handles.Add( nameHandle );
            var namePtr = new hkStringPtr {
                StringAndFlag = ( byte* )nameHandle
            };
            Binding->OriginalSkeletonName = namePtr;
            Binding->BlendHint.Storage = ( sbyte )BlendHint.Value;
        }

        public HashSet<int> GetUnanimatedBones() {
            var animatedBones = new HashSet<int>();

            for( var i = 0; i < Binding->TransformTrackToBoneIndices.Length; i++ ) {
                animatedBones.Add( Binding->TransformTrackToBoneIndices[i] );
            }

            var unanimatedBones = new HashSet<int>();
            for( var i = 0; i < Skeleton->Bones.Length; i++ ) {
                if( !animatedBones.Contains( i ) ) unanimatedBones.Add( i );
            }
            return unanimatedBones;
        }

        // sampleAndCombineAnimations uses the binding's track-to-bone indices to address the
        // skeleton's own internal arrays (Bones, ReferencePose, ParentIndices, FloatSlots).
        // Those arrays are only Skeleton->Bones.Length / FloatSlots.Length long, so any index
        // past the end reads garbage from adjacent heap memory and may also write blended
        // results into skeleton-internal pose buffers sized for the real bone count. The heap
        // corruption then surfaces seconds later when the game's own Framework.Tick touches
        // the damaged memory, producing a crash in the GAME's call stack (not the plugin's).
        // We must refuse to sample when the skeleton doesn't fit the animation.

        public bool HasMismatchedSkeleton() {
            var numBones = Skeleton->Bones.Length;
            for( var i = 0; i < Binding->TransformTrackToBoneIndices.Length; i++ ) {
                var idx = Binding->TransformTrackToBoneIndices[i];
                if( idx >= 0 && idx >= numBones ) return true;
            }
            var numFloats = Skeleton->FloatSlots.Length;
            for( var i = 0; i < Binding->FloatTrackToFloatSlotIndices.Length; i++ ) {
                var idx = Binding->FloatTrackToFloatSlotIndices[i];
                if( idx >= 0 && idx >= numFloats ) return true;
            }
            return false;
        }

        private short[] ClampedBoneIndices;
        private short[] ClampedFloatIndices;

        // Animations made for skeletons with more bones than the one the game will use reference bone
        // indices past the end of that skeleton. The game writes to these indices directly when sampling
        // the animation, so out-of-range indices corrupt memory and crash the game. They are temporarily
        // removed before the animation is serialized, and restored afterwards so the original data is kept

        public void ClampTrackIndices() {
            var numBones = Skeleton->Bones.Length;
            var numFloats = Skeleton->FloatSlots.Length;

            ClampedBoneIndices = new short[Binding->TransformTrackToBoneIndices.Length];
            for( var i = 0; i < Binding->TransformTrackToBoneIndices.Length; i++ ) {
                ClampedBoneIndices[i] = Binding->TransformTrackToBoneIndices[i];
                var idx = ClampedBoneIndices[i];
                if( idx < 0 || idx >= numBones ) Binding->TransformTrackToBoneIndices[i] = -1;
            }

            ClampedFloatIndices = new short[Binding->FloatTrackToFloatSlotIndices.Length];
            for( var i = 0; i < Binding->FloatTrackToFloatSlotIndices.Length; i++ ) {
                ClampedFloatIndices[i] = Binding->FloatTrackToFloatSlotIndices[i];
                var idx = ClampedFloatIndices[i];
                if( idx < 0 || idx >= numFloats ) Binding->FloatTrackToFloatSlotIndices[i] = -1;
            }
        }

        public void UnclampTrackIndices() {
            var numBones = Skeleton->Bones.Length;
            var numFloats = Skeleton->FloatSlots.Length;

            if( ClampedBoneIndices != null ) {
                for( var i = 0; i < ClampedBoneIndices.Length; i++ ) Binding->TransformTrackToBoneIndices[i] = ClampedBoneIndices[i];
            }
            if( ClampedFloatIndices != null ) {
                for( var i = 0; i < ClampedFloatIndices.Length; i++ ) Binding->FloatTrackToFloatSlotIndices[i] = ClampedFloatIndices[i];
            }
        }

        public void Dispose() {
            AnimatedSkeleton->removeAnimationControl( AnimationControl );
            Marshal.FreeHGlobal( ( nint )AnimatedSkeleton );
            Marshal.FreeHGlobal( ( nint )AnimationControl );
        }
    }
}
