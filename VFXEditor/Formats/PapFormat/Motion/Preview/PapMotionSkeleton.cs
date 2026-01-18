using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX.Animations;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.SkeletonBuilder;
using VfxEditor.PapFormat;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Utils;
using VfxEditor.DirectX;

namespace VfxEditor.Formats.PapFormat.Motion.Preview {
    public unsafe class PapMotionSkeleton : PapMotionPreview {
        public readonly int RenderId = RenderInstance.NewId;
        public readonly PapFile File;

        private List<Bone> Data;
        private int Frame = 0;
        private bool Looping = true;
        private bool Playing = false;
        private DateTime LastTime = DateTime.Now;

        public PapMotionSkeleton( PapFile file, PapMotion motion ) : base( motion ) {
            File = file;
        }

        public override void Draw( int idx ) {
            if( Data == null ) { // Init
                UpdateRender();
            }
            else if( File.BoneInstance.CurrentRenderId != RenderId ) { // Just switched
                Frame = 0;
                Playing = false;
            }

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

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
                if( Frame >= Motion.TotalFrames ) Frame = Motion.TotalFrames - 1;
            }

            // ==========

            if( Data == null ) return;

            if( Playing ) {
                var time = DateTime.Now;
                var diff = ( time - LastTime ).TotalMilliseconds;
                if( diff > 33.3f ) {
                    LastTime = time;

                    Frame++;
                    if( Frame >= Motion.TotalFrames ) {
                        if( !Looping ) { // Stop
                            Frame = Motion.TotalFrames - 1;
                            Playing = false;
                        }
                        else { // Loop back around
                            Frame = 0;
                        }
                    }
                }
            }

            if( Frame != lastFrame ) UpdateRender();

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 5 );
            if( ImGui.Button( "Export" ) ) ExportDialog( Motion.File.Animations[idx].GetName() );
            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) ImportDialog( idx );
            ImGui.SameLine();
            UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );

            Plugin.DirectXManager.BoneRenderer.DrawTexture( RenderId, File.BoneInstance, UpdateRender, Plugin.Configuration.DrawDirectXSkeleton );
        }

        // ======== UPDATING ===========

        private void UpdateRender() {
            Motion.AnimationControl->LocalTime = Frame * ( 1 / 30f );

            var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( Motion.Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
            var floats = ( float* )Marshal.AllocHGlobal( Motion.Skeleton->FloatSlots.Length * sizeof( float ) );
            Motion.AnimatedSkeleton->sampleAndCombineAnimations( transforms, floats );

            Data = [];

            var parents = new List<int>();
            var refPoses = new List<Matrix4x4>();
            var bindPoses = new List<Matrix4x4>();

            for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                var transform = transforms[i];
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                var matrix = HavokUtils.CleanMatrix( MatrixHelper.AffineTransformation(
                    scl.X,
                    new Quaternion( rot.X, rot.Y, rot.Z, rot.W ),
                    new Vector3( pos.X, pos.Y, pos.Z )
                ) );

                parents.Add( Motion.Skeleton->ParentIndices[i] );
                refPoses.Add( matrix );
                bindPoses.Add( Matrix4x4.Identity );
            }

            for( var target = 0; target < Motion.Skeleton->Bones.Length; target++ ) {
                var current = target;
                while( current >= 0 ) {
                    bindPoses[target] = Matrix4x4.Multiply( bindPoses[target], refPoses[current] );
                    current = parents[current];
                }
            }

            for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                var bone = new Bone {
                    BindPose = bindPoses[i],
                    ParentIndex = parents[i],
                    Name = Motion.Skeleton->Bones[i].Name.String
                };

                Data.Add( bone );
            }

            Marshal.FreeHGlobal( ( nint )transforms );
            Marshal.FreeHGlobal( ( nint )floats );

            if( Data == null || Data.Count == 0 || Motion.TotalFrames == 0 ) {
                Plugin.DirectXManager.BoneRenderer.SetEmpty( RenderId, File.BoneInstance );
            }
            else {
                Plugin.DirectXManager.BoneRenderer.SetSkeleton( RenderId, File.BoneInstance, new ConnectedSkeletonMeshBuilder( Data, -1, Motion.GetUnanimatedBones() ).Build() );
            }
        }
    }
}
