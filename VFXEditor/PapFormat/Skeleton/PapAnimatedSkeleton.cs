using Dalamud.Interface;
using FFXIVClientStructs.Havok;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.DirectX;
using VfxEditor.Interop.Havok;

namespace VfxEditor.PapFormat.Skeleton {
    public unsafe class PapAnimatedSkeleton {
        private readonly PapFile File;
        private readonly hkaAnimatedSkeleton* Skeleton;
        private readonly hkaAnimationControl* Animation;

        private float Time => Animation->LocalTime;
        private float Duration => Animation->Binding.ptr->Animation.ptr->Duration;
        private int TotalFrames => ( int )( Duration * 30f );

        private static PapPreview PapPreview => Plugin.DirectXManager.PapPreview;

        private List<Bone> Data;
        private int Frame = 0;
        private bool Looping = true;
        private bool Playing = false;
        private DateTime LastTime = DateTime.Now;

        public PapAnimatedSkeleton( PapFile file, HavokData bones, hkaAnimationBinding* binding ) {
            File = file;
            Skeleton = ( hkaAnimatedSkeleton* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimatedSkeleton ) ) );
            Animation = ( hkaAnimationControl* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimationControl ) ) );

            Animation->Ctor1( binding );
            Skeleton->Ctor1( bones.AnimationContainer->Skeletons[0].ptr );
            Skeleton->addAnimationControl( Animation );

            UpdateFrameData();
        }

        public void Draw() {
            if( Data == null ) {
                UpdateFrameData();
            }
            else if( PapPreview.CurrentAnimation != this ) {
                Frame = 0;
                Playing = false;
                UpdateFrameData();
            }

            // ==== Frame controls ====

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( Playing ? FontAwesomeIcon.Stop.ToIconString() : FontAwesomeIcon.Play.ToIconString() ) ) Playing = !Playing;

                ImGui.SameLine();
                using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, !Looping );
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) Looping = !Looping;
            }

            var lastFrame = Frame;

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Frame", ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                if( Frame >= TotalFrames ) Frame = TotalFrames - 1;
            }

            // ==========

            if( Data == null ) return;

            if( Playing ) {
                var time = DateTime.Now;
                var diff = ( time - LastTime ).TotalMilliseconds;
                if( diff > 33.3f ) {
                    LastTime = time;

                    Frame++;
                    if( Frame >= TotalFrames ) {
                        if( !Looping ) { // Stop
                            Frame = TotalFrames - 1;
                            Playing = false;
                        }
                        else { // Loop back around
                            Frame = 0;
                        }
                    }
                }
            }

            if( Frame != lastFrame ) UpdateFrameData();

            PapPreview.DrawInline();
        }

        private void UpdateFrameData() {
            Animation->LocalTime = Frame * ( 1 / 30f );

            var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( Skeleton->Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
            var floats = ( float* )Marshal.AllocHGlobal( Skeleton->Skeleton->FloatSlots.Length * sizeof( float ) );
            Skeleton->sampleAndCombineAnimations( transforms, floats );

            Data = new();

            var parents = new List<int>();
            var refPoses = new List<Matrix>();
            var bindPoses = new List<Matrix>();

            for( var i = 0; i < Skeleton->Skeleton->Bones.Length; i++ ) {
                var transform = transforms[i];
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                var matrix = AnimationData.CleanMatrix( Matrix.AffineTransformation(
                    scl.X,
                    new Quaternion( rot.X, rot.Y, rot.Z, rot.W ),
                    new Vector3( pos.X, pos.Y, pos.Z )
                ) );

                parents.Add( Skeleton->Skeleton->ParentIndices[i] );
                refPoses.Add( matrix );
                bindPoses.Add( Matrix.Identity );
            }

            for( var target = 0; target < Skeleton->Skeleton->Bones.Length; target++ ) {
                var current = target;
                while( current >= 0 ) {
                    bindPoses[target] = Matrix.Multiply( bindPoses[target], refPoses[current] );
                    current = parents[current];
                }
            }

            for( var i = 0; i < Skeleton->Skeleton->Bones.Length; i++ ) {
                var bone = new Bone {
                    BindPose = bindPoses[i],
                    ParentIndex = parents[i],
                    Name = Skeleton->Skeleton->Bones[i].Name.String
                };

                Data.Add( bone );
            }

            Marshal.FreeHGlobal( ( nint )transforms );
            Marshal.FreeHGlobal( ( nint )floats );

            UpdatePreview();
        }

        private void UpdatePreview() {
            if( Data == null || Data.Count == 0 || TotalFrames == 0 ) {
                PapPreview.LoadEmpty( File, this );
            }
            else {
                PapPreview.LoadSkeleton( File, this, AnimationData.CreateSkeletonMesh( Data, -1 ) );
            }
        }

        public void Dispose() {
            Skeleton->Dtor();
            Marshal.FreeHGlobal( ( nint )Skeleton );
            Marshal.FreeHGlobal( ( nint )Animation );
            if( PapPreview.CurrentAnimation == this ) PapPreview.ClearAnimation();
        }
    }
}
