using Dalamud.Interface;
using FFXIVClientStructs.Havok;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.Interop.Havok;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat.Motion {
    public unsafe class PapMotions : HavokData {
        public readonly PapFile File;
        public readonly string SklbTempPath;
        public readonly List<PapMotion> Motions = new();

        public HavokData Bones;
        public hkaSkeleton* Skeleton => Bones.AnimationContainer->Skeletons[0].ptr;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";
        private bool SklbReplaced = false;

        public PapMotions( PapFile file, string havokPath ) : base( havokPath ) {
            File = file;
            SklbTempPath = Path.Replace( ".hkx", "_sklb.hkx" );

            // Initialize skeleton using best guess for sklb path
            var sklbPath = GetSklbPath();
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;

            Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var replaced );
            SklbReplaced = replaced;
            UpdateSkeleton( simple );
        }

        public void UpdateMotions() {
            Motions.ForEach( x => x.Dispose() );
            Motions.Clear();

            for( var i = 0; i < AnimationContainer->Bindings.Length; i++ ) {
                Motions.Add( new( File, Bones, AnimationContainer->Bindings[i].ptr ) );
            }
        }

        private void UpdateSkeleton( SimpleSklb sklbFile ) {
            Bones?.RemoveReference();
            sklbFile.SaveHavokData( SklbTempPath );
            Bones = new( SklbTempPath );

            UpdateMotions();
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
            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Sync );
            var inputSize = ImGui.GetContentRegionAvail().X - 400;
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) {
                        if( Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var replaced ) ) {
                            SklbReplaced = replaced;
                            UpdateSkeleton( simple );
                        }
                    }

                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                        FileDialogManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                            if( !ok ) return;
                            SklbReplaced = false;
                            UpdateSkeleton( SimpleSklb.LoadFromLocal( res ) );
                        } );
                    }
                }

                if( SklbReplaced ) {
                    ImGui.SameLine();
                    ImGui.TextColored( UiUtils.GREEN_COLOR, "Replaced" );
                }
            }

            Motions[havokIndex].Draw( havokIndex );
        }

        public void DrawHavok( int havokIndex ) {
            Motions[havokIndex].DrawHavok();
        }

        public void Write() {
            var handles = new List<nint>();
            Motions.ForEach( x => x.UpdateHavok( handles ) );
            WriteHavok();
            handles.ForEach( Marshal.FreeHGlobal );
        }

        public void Dispose() {
            Motions.ForEach( x => x.Dispose() );
        }
    }
}
