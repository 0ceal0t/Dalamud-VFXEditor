using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using VfxEditor.Interop;
using VfxEditor.PapFormat;

namespace VfxEditor.Animation {
    public class AnimationManager {
        private bool Looping = true;
        private bool Playing = false;

        private static string SklHkxTemp => Path.Combine( Plugin.Configuration.WriteLocation, "skl_temp.hkx" ).Replace( '\\', '/' );
        private static string BinTemp => Path.Combine( Plugin.Configuration.WriteLocation, "anim_out.bin" ).Replace( '\\', '/' );

        private bool AnimationLoaded = false;
        private AnimationData Data = null;
        private int Frame = 0;
        private string LastSklbPath = string.Empty;
        private PapAnimation SelectedAnimation = null;

        private DateTime LastFrame = DateTime.Now;

        public void Load( string animationHkx, int animationIndex, string sklbPath ) {
            // Reset
            Frame = 0;
            LastSklbPath = sklbPath;

            if( Plugin.DataManager.FileExists( sklbPath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile<SklbFile>( sklbPath );
                    file.SaveHavokData( SklHkxTemp );
                    HavokInterop.HavokToBin( animationHkx, animationIndex, SklHkxTemp, BinTemp );
                    Data = new AnimationData( BinTemp );
                    Plugin.DirectXManager.AnimationPreview.LoadSkeleton( Data.GetBoneMesh( 0 ) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, $"Could not read file: {sklbPath}" );
                    return;
                }
            }
            else PluginLog.Error( $"Could not find file: {sklbPath}" );
            AnimationLoaded = true;
        }

        public void Draw( PapAnimation animation, string animationHkx, int animationIndex ) {
            if( animation != SelectedAnimation ) {
                // Selected a new animation, reset
                SelectedAnimation = animation;
                AnimationLoaded = false;
                Playing = false;
                return;
            }

            if( !AnimationLoaded ) {
                if( ImGui.Button( "Load Animation" ) ) {
                    Playing = false;
                    Load( animationHkx, animationIndex, GetSklbPath( animation ) );
                }
                return;
            }

            if( ImGui.Button( "Refresh" ) ) {
                Playing = false;
                Load( animationHkx, animationIndex, GetSklbPath( animation ) );
            }

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

                    Plugin.DirectXManager.AnimationPreview.LoadSkeleton( Data.GetBoneMesh( Frame ) );
                }
            }

            // Controls

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( Playing ? FontAwesomeIcon.Stop.ToIconString() : FontAwesomeIcon.Play.ToIconString() ) ) Playing = !Playing;

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );

                using var dimmed = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.5f, !Looping );
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) Looping = !Looping;
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Frame", ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                if( Frame >= Data.NumFrames ) Frame = Data.NumFrames - 1;

                Plugin.DirectXManager.AnimationPreview.LoadSkeleton( Data.GetBoneMesh( Frame ) );
            }

            ImGui.TextDisabled( LastSklbPath );

            Plugin.DirectXManager.AnimationPreview.DrawInline();
        }

        private static string GetSklbPath( PapAnimation animation ) {
            var modelType = animation.File.ModelType.Value;
            var modelId = animation.File.ModelId.Value;
            var sourcePath = animation.File.SourcePath;

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

        public void Dispose() {
            SelectedAnimation = null;
            if( File.Exists( SklHkxTemp ) ) File.Delete( SklHkxTemp );
            if( File.Exists( BinTemp ) ) File.Delete( BinTemp );
        }
    }
}
