using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using VfxEditor.Animation;
using VfxEditor.DirectX;
using VfxEditor.Interop;
using VfxEditor.Utils;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.PapFormat.Skeleton {
    public class SkeletonView {
        private readonly PapFile File;
        private readonly PapAnimation Animation;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";

        private static PapPreview PapPreview => Plugin.DirectXManager.PapPreview;
        private static string SklHkxTemp => Path.Combine( Plugin.Configuration.WriteLocation, "skl_temp.hkx" ).Replace( '\\', '/' );
        private static string BinTemp => Path.Combine( Plugin.Configuration.WriteLocation, "anim_out.bin" ).Replace( '\\', '/' );

        private AnimationData Data;
        private int Frame = 0;
        private bool Looping = true;
        private bool Playing = false;
        private DateTime LastFrame = DateTime.Now;

        public SkeletonView( PapFile file, PapAnimation animation ) {
            File = file;
            Animation = animation;
            var sklbPath = GetSklbPath();
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;
        }

        public void Draw() {
            if( Data == null ) LoadSklbPath();
            else if( PapPreview.CurrentAnimation != Animation ) {
                Frame = 0;
                Playing = false;
                UpdateSkeleton();
            }

            // ==== Skeleton ====

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Sync );
            var inputSize = UiUtils.GetOffsetInputSize( checkSize );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vec2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) LoadSklbPath();

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileDialogManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        UpdateData( SklbFile.LoadFromLocal( res ) );
                        UpdateSkeleton();
                    } );
                }
            }

            ImGui.SameLine();
            UiUtils.HelpMarker( @"This skeleton is for preview purposes only, and does not affect the physics file" );

            // ==== Frame controls ====

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( Playing ? FontAwesomeIcon.Stop.ToIconString() : FontAwesomeIcon.Play.ToIconString() ) ) Playing = !Playing;

                ImGui.SameLine();
                using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, !Looping );
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) Looping = !Looping;
            }

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Frame", ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                if( Frame >= Data.NumFrames ) Frame = Data.NumFrames - 1;
            }

            // ==========

            if( Data == null ) return;

            if( Playing ) {
                var time = DateTime.Now;
                var diff = ( time - LastFrame ).TotalMilliseconds;
                if( diff > 33.3f ) { // 30 fps (1000ms / 30)
                    LastFrame = time;

                    Frame++;
                    if( Frame >= Data.NumFrames ) {
                        if( !Looping ) { // Stop
                            Frame = Data.NumFrames - 1;
                            Playing = false;
                        }
                        else { // Loop back around
                            Frame = 0;
                        }
                    }

                    UpdateSkeleton();
                }
            }

            PapPreview.DrawInline();
        }

        private string GetSklbPath() {
            var modelType = File.ModelType.Value;
            var modelId = File.ModelId.Value;
            var sourcePath = File.SourcePath;

            if( sourcePath.Contains( "animation/f" ) ) {
                // chara / human / c1301 / animation / f0002 /nonresident/emot/airquotes.pap
                var split = sourcePath.Split( '/' );
                var charaType = split[2];
                var faceType = split[4];
                return $"chara/human/{charaType}/skeleton/face/{faceType}/skl_{charaType}{faceType}.sklb";
            }

            var format = modelType switch {
                SkeletonType.Monster => "chara/monster/m{0:D4}/skeleton/base/b{1:D4}/skl_m{0:D4}b{1:D4}.sklb",
                SkeletonType.DemiHuman => "chara/demihuman/d{0:D4}/skeleton/base/b{1:D4}/skl_d{0:D4}b{1:D4}.sklb",
                SkeletonType.Human => "chara/human/c{0:D4}/skeleton/base/b{1:D4}/skl_c{0:D4}b{1:D4}.sklb",
                SkeletonType.Weapon => "chara/weapon/w{0:D4}/skeleton/base/b{1:D4}/skl_w{0:D4}b{1:D4}.sklb",
                _ => ""
            };

            return string.Format( format, modelId, 1 ); // TODO: is this always 1?
        }

        private void LoadSklbPath() {
            if( Plugin.DataManager.FileExists( SklbPreviewPath ) ) {
                UpdateData( Plugin.DataManager.GetFile<SklbFile>( SklbPreviewPath ) );
                UpdateSkeleton();
            }
            else {
                PluginLog.Error( $"File does not exist: {SklbPreviewPath}" );
            }
        }

        private void UpdateData( SklbFile sklbFile ) {
            try {
                sklbFile.SaveHavokData( SklHkxTemp );
                HavokInterop.HavokToBin( Animation.HkxTempLocation, Animation.HavokIndex, SklHkxTemp, BinTemp );
                Data = new AnimationData( BinTemp );
            }
            catch( Exception ) {
                PluginLog.Error( "Could not load SKLB file" );
                return;
            }
        }

        private void UpdateSkeleton() {
            if( Data == null || Data.NumFrames == 0 ) PapPreview.LoadEmpty( File, Animation );
            else {
                PapPreview.LoadSkeleton( File, Animation, Data.GetBoneMesh( Frame ) );
            }
        }

        public void ClearData() {
            if( PapPreview.CurrentAnimation == Animation ) PapPreview.ClearAnimation();
            Data = null;
        }

        // TODO: make sure things are cleaned up
        public void Dispose() {
            if( PapPreview.CurrentAnimation == Animation ) PapPreview.ClearAnimation();
        }
    }
}
