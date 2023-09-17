using Dalamud.Interface;
using FFXIVClientStructs.Havok;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.DirectX;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.SkeletonBuilder;
using VfxEditor.Parsing;
using VfxEditor.Utils;

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

        private static PapPreview PapPreview => Plugin.DirectXManager.PapPreview;

        private List<Bone> Data;
        private int Frame = 0;
        private bool Looping = true;
        private bool Playing = false;
        private DateTime LastTime = DateTime.Now;

        private readonly ParsedString OriginalSkeletonName = new( "Original Skeleton Name" );
        private readonly ParsedEnum<BlendHintTypes> BlendHint = new( "Blend Hint" );

        public PapMotion( PapFile file, HavokData bones, hkaAnimationBinding* binding ) {
            File = file;
            AnimatedSkeleton = ( hkaAnimatedSkeleton* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimatedSkeleton ) ) );
            AnimationControl = ( hkaAnimationControl* )Marshal.AllocHGlobal( Marshal.SizeOf( typeof( hkaAnimationControl ) ) );

            AnimationControl->Ctor1( binding );
            AnimatedSkeleton->Ctor1( bones.AnimationContainer->Skeletons[0].ptr );
            AnimatedSkeleton->addAnimationControl( AnimationControl );

            OriginalSkeletonName.Value = Binding->OriginalSkeletonName.String;
            BlendHint.Value = ( BlendHintTypes )Binding->BlendHint.Value;
        }

        // ======= DRAWING =========

        public void Draw( int idx ) {
            if( Data == null ) {
                UpdateFrameData();
            }
            else if( PapPreview.CurrentMotion != this ) {
                Frame = 0;
                Playing = false;
                UpdateFrameData();
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

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 5 );
            if( ImGui.Button( "Export" ) ) ExportDialog( File.Animations[idx].GetName() );

            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) ImportDialog( idx );

            ImGui.SameLine();
            UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );


            PapPreview.DrawInline();
        }

        // ======== OTHER HAVOK STUFF ==========

        public void DrawHavok() {
            ImGui.TextDisabled( $"{Animation->Type}" );
            OriginalSkeletonName.Draw( CommandManager.Pap );
            BlendHint.Draw( CommandManager.Pap );
        }

        public void UpdateHavok( List<nint> handles ) {
            var nameHandle = Marshal.StringToHGlobalAnsi( OriginalSkeletonName.Value );
            handles.Add( nameHandle );
            var namePtr = new hkStringPtr {
                StringAndFlag = ( byte* )nameHandle
            };
            Binding->OriginalSkeletonName = namePtr;
            Binding->BlendHint.Storage = ( sbyte )BlendHint.Value;
        }

        // ======== IMPORT EXPORT =========

        private void ExportDialog( string animationName ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".gltf", "motion", "gltf", ( bool ok, string res ) => {
                if( !ok ) return;
                Plugin.AddModal( new PapGltfExportModal( this, animationName, res ) );
            } );
        }

        private void ImportDialog( int idx ) {
            FileDialogManager.OpenFileDialog( "Select a File", "Motion{.hkx,.gltf},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                if( res.Contains( ".hkx" ) ) {
                    Plugin.AddModal( new PapReplaceModal( this, idx, res ) );
                }
                else {
                    Plugin.AddModal( new PapGltfImportModal( this, idx, res ) );
                }
            } );
        }

        // ======== UPDATING ===========

        private void UpdateFrameData() {
            AnimationControl->LocalTime = Frame * ( 1 / 30f );

            var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
            var floats = ( float* )Marshal.AllocHGlobal( Skeleton->FloatSlots.Length * sizeof( float ) );
            AnimatedSkeleton->sampleAndCombineAnimations( transforms, floats );

            Data = [];

            var parents = new List<int>();
            var refPoses = new List<Matrix>();
            var bindPoses = new List<Matrix>();

            for( var i = 0; i < Skeleton->Bones.Length; i++ ) {
                var transform = transforms[i];
                var pos = transform.Translation;
                var rot = transform.Rotation;
                var scl = transform.Scale;

                var matrix = HavokUtils.CleanMatrix( Matrix.AffineTransformation(
                    scl.X,
                    new Quaternion( rot.X, rot.Y, rot.Z, rot.W ),
                    new Vector3( pos.X, pos.Y, pos.Z )
                ) );

                parents.Add( Skeleton->ParentIndices[i] );
                refPoses.Add( matrix );
                bindPoses.Add( Matrix.Identity );
            }

            for( var target = 0; target < Skeleton->Bones.Length; target++ ) {
                var current = target;
                while( current >= 0 ) {
                    bindPoses[target] = Matrix.Multiply( bindPoses[target], refPoses[current] );
                    current = parents[current];
                }
            }

            for( var i = 0; i < Skeleton->Bones.Length; i++ ) {
                var bone = new Bone {
                    BindPose = bindPoses[i],
                    ParentIndex = parents[i],
                    Name = Skeleton->Bones[i].Name.String
                };

                Data.Add( bone );
            }

            Marshal.FreeHGlobal( ( nint )transforms );
            Marshal.FreeHGlobal( ( nint )floats );

            UpdatePreview();
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

        private void UpdatePreview() {
            if( Data == null || Data.Count == 0 || TotalFrames == 0 ) {
                PapPreview.LoadEmpty( File, this );
            }
            else {
                PapPreview.LoadSkeleton( File, this, new ConnectedSkeletonMeshBuilder( Data, -1, GetUnanimatedBones() ).Build() );
            }
        }

        public void Dispose() {
            AnimatedSkeleton->removeAnimationControl( AnimationControl );
            // if( Data != null ) Skeleton->Dtor(); // Sometimes causes crashes. idk
            Marshal.FreeHGlobal( ( nint )AnimatedSkeleton );
            Marshal.FreeHGlobal( ( nint )AnimationControl );
            if( PapPreview.CurrentMotion == this ) PapPreview.ClearAnimation();
        }
    }
}
