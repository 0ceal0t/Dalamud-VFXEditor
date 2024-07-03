using FFXIVClientStructs.Havok.Animation.Animation;
using FFXIVClientStructs.Havok.Animation.Playback;
using FFXIVClientStructs.Havok.Animation.Playback.Control;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using ImGuiNET;
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
            AnimatedSkeleton = ( hkaAnimatedSkeleton* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimatedSkeleton ) ) );
            AnimationControl = ( hkaAnimationControl* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimationControl ) ) );

            AnimationControl->Ctor1( binding );
            AnimatedSkeleton->Ctor1( bones.AnimationContainer->Skeletons[0].ptr );
            AnimatedSkeleton->addAnimationControl( AnimationControl );

            OriginalSkeletonName.Value = Binding->OriginalSkeletonName.String;
            BlendHint.Value = ( BlendHintTypes )Binding->BlendHint.Value;

            Preview = file.IsMaterial ? new PapMotionMaterial( this ) : new PapMotionSkeleton( this );
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

        public void Dispose() {
            AnimatedSkeleton->removeAnimationControl( AnimationControl );
            Marshal.FreeHGlobal( ( nint )AnimatedSkeleton );
            Marshal.FreeHGlobal( ( nint )AnimationControl );
        }
    }
}
