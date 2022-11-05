using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor;
using VfxEditor.Interop;
using VfxEditor.PapFormat;
using VfxEditor.Utils;

namespace VFXEditor.Animation {
    public class AnimationManager {
        public enum SkeletonType {
            Human,
            Monster,
            DemiHuman
        }

        private SkeletonType SelectedSkeletonType = SkeletonType.Human;
        private static readonly SkeletonType[] SkeletonOptions = new[] { SkeletonType.Human, SkeletonType.Monster, SkeletonType.DemiHuman };

        private bool Looping = true;
        private bool Playing = false;

        private static string SklHkxTemp => Path.Combine( Plugin.Configuration.WriteLocation, $"skl_temp.hkx" ).Replace( '\\', '/' );
        private static string BinTemp => Path.Combine( Plugin.Configuration.WriteLocation, $"anim_out.bin" ).Replace( '\\', '/' );

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
                    Plugin.DirectXManager.AnimationPreview.LoadAnimation( Data.GetBoneMesh( 0) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, $"Could not read file: {sklbPath}" );
                    return;
                }
            }
            else PluginLog.Error( $"Could not find file: {sklbPath}" );
            AnimationLoaded = true;
        }

        public void Draw( PapAnimation animation, string animationHkx, int animationIndex, short modelId, byte baseId, byte variantId ) {
            if( animation != SelectedAnimation ) {
                // Selected a new animation, reset
                SelectedAnimation = animation;
                AnimationLoaded = false;
                Playing = false;
                return;
            }

            if( !AnimationLoaded ) {
                ImGui.SetNextItemWidth( 100f );
                if (UiUtils.EnumComboBox("Type", SkeletonOptions, SelectedSkeletonType, out var newSkeletonType )) {
                    SelectedSkeletonType= newSkeletonType;
                }
                ImGui.SameLine();
                if( ImGui.Button( "Load" ) ) {
                    Playing = false;
                    Load( animationHkx, animationIndex, GetSklbPath( modelId, baseId, variantId) );
                }

                return;
            }

            if (ImGui.Button( "Refresh") ) {
                Playing = false;
                Load( animationHkx, animationIndex, GetSklbPath( modelId, baseId, variantId ) );
            }

            if( Playing ) {
                var time = DateTime.Now;
                var diff = ( time - LastFrame ).TotalMilliseconds;
                if( diff > 33.3f ) { // 30 fps
                    LastFrame = time;

                    Frame++;
                    if( Frame >= Data.NumFrames ) {
                        if( !Looping ) {
                            // Stop
                            Frame = Data.NumFrames - 1;
                            Playing = false;
                        }
                        else {
                            // Loop back around
                            Frame = 0;
                        }
                    }

                    Plugin.DirectXManager.AnimationPreview.LoadAnimation( Data.GetBoneMesh( Frame ) );
                }
            }

            // Controls

            ImGui.PushFont( UiBuilder.IconFont );

            ImGui.SameLine();
            if( ImGui.Button(Playing ? $"{( char )FontAwesomeIcon.Stop}" : $"{( char )FontAwesomeIcon.Play}" ) ) Playing = !Playing;

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            var dimLoop = !Looping;
            if( dimLoop ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Sync}" ) ) Looping = !Looping;
            if( dimLoop ) ImGui.PopStyleVar();

            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Frame", ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                if( Frame >= Data.NumFrames ) Frame = Data.NumFrames - 1;

                Plugin.DirectXManager.AnimationPreview.LoadAnimation( Data.GetBoneMesh( Frame ) );
            }

            ImGui.TextDisabled( LastSklbPath );

            // ============

            var cursor = ImGui.GetCursorScreenPos();
            ImGui.BeginChild( "AnimationChild" );

            var space = ImGui.GetContentRegionAvail();
            Plugin.DirectXManager.AnimationPreview.Resize( space );

            ImGui.ImageButton( Plugin.DirectXManager.AnimationPreview.Output, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );

            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                Plugin.DirectXManager.AnimationPreview.Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Plugin.DirectXManager.AnimationPreview.Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                Plugin.DirectXManager.AnimationPreview.IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) {
                Plugin.DirectXManager.AnimationPreview.Zoom( ImGui.GetIO().MouseWheel );
            }

            ImGui.EndChild();
        }

        // chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb
        // chara/monster/m0101/skeleton/base/b0001/skl_m0101b0001.sklb <--- is there a way to tell these apart?

        private string GetSklbPath( short modelId, byte baseId, byte variantId ) {
            var format = SelectedSkeletonType switch {
                SkeletonType.Monster => "chara/monster/m{0:D4}/skeleton/base/b{1:D4}/skl_m{0:D4}b{1:D4}.sklb",
                SkeletonType.DemiHuman => "chara/demihuman/d{0:D4}/skeleton/base/b{1:D4}/skl_d{0:D4}b{1:D4}.sklb",
                SkeletonType.Human => "chara/human/c{0:D4}/skeleton/base/b{1:D4}/skl_c{0:D4}b{1:D4}.sklb",
                _ => ""
            };

            return string.Format( format, modelId, 1 );
        }

        public void Dispose() {
            SelectedAnimation = null;
            if( File.Exists( SklHkxTemp ) ) File.Delete( SklHkxTemp );
            if( File.Exists( BinTemp ) ) File.Delete( BinTemp );
        }
    }
}
