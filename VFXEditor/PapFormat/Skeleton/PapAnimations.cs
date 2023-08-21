using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Interop.Havok;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat.Skeleton {
    public unsafe class PapAnimations : HavokData {
        private readonly PapFile File;
        private readonly string SklbTempPath;
        private HavokData Bones;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";
        public readonly List<PapAnimatedSkeleton> Animations = new();

        public PapAnimations( PapFile file, string havokPath ) : base( havokPath ) {
            File = file;
            SklbTempPath = Path.Replace( ".hkx", "_sklb.hkx" );

            // Initialize skeleton using best guess for sklb path
            var sklbPath = GetSklbPath();
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;

            UpdateSkeleton( Plugin.DataManager.GetFile<SimpleSklb>( SklbPreviewPath ) );
        }

        public void UpdateAnimations() {
            Animations.ForEach( x => x.Dispose() );
            Animations.Clear();

            for( var i = 0; i < AnimationContainer->Bindings.Length; i++ ) {
                Animations.Add( new( File, Bones, AnimationContainer->Bindings[i].ptr ) );
            }
        }

        private void UpdateSkeleton( SimpleSklb sklbFile ) {
            Bones?.RemoveReference();
            sklbFile.SaveHavokData( SklbTempPath );
            Bones = new( SklbTempPath );

            UpdateAnimations();
        }

        private string GetSklbPath() {
            var modelType = File.ModelType.Value;
            var modelId = File.ModelId.Value;
            var sourcePath = File.SourcePath;

            if( !string.IsNullOrEmpty( sourcePath ) && sourcePath.Contains( "animation/f" ) ) {
                // chara/human/c1301/animation/f0002/nonresident/emot/airquotes.pap
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

        public void Draw( int havokIndex ) {
            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
            var inputSize = UiUtils.GetOffsetInputSize( checkSize );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) {
                    if( Plugin.DataManager.FileExists( SklbPreviewPath ) ) UpdateSkeleton( Plugin.DataManager.GetFile<SimpleSklb>( SklbPreviewPath ) );
                }

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileDialogManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        UpdateSkeleton( SimpleSklb.LoadFromLocal( res ) );
                    } );
                }
            }

            ImGui.SameLine();
            UiUtils.HelpMarker( @"This skeleton is for preview purposes only, and does not affect the animation file" );

            Animations[havokIndex].Draw();
        }

        public void Write() {
            WriteHavok();
        }

        public void Dispose() {
            Animations.ForEach( x => x.Dispose() );
            Bones?.RemoveReference();
            RemoveReference();
        }
    }
}
