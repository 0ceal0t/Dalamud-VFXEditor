using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.DirectX;
using VfxEditor.FileManager;
using VfxEditor.Interop.Havok.SkeletonBuilder;

namespace VfxEditor.Interop.Havok.Ui {
    public abstract class SkeletonView {
        private readonly string Extension;
        private readonly FileManagerFile File;

        protected readonly BoneNamePreview Preview;
        protected HavokBones Bones;
        protected readonly SkeletonSelector Selector;

        public SkeletonView( FileManagerFile file, BoneNamePreview preview, string sklbPath, string extension ) {
            File = file;
            Preview = preview;
            Extension = extension;
            if( !string.IsNullOrEmpty( sklbPath ) ) sklbPath = sklbPath.Replace( $".{extension}", ".sklb" ).Replace( $"{extension[0..3]}_", "skl_" );
            Selector = new( sklbPath, UpdateSkeleton );
        }

        public void Draw() {
            if( Bones == null ) {
                Selector.Init();
            }
            else if( Preview.CurrentFile != File ) {
                UpdatePreview();
                UpdateData();
            }

            Selector.Draw();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
                if( ImGui.Checkbox( "Show Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();
            }

            if( Bones == null ) return;
            DrawData();
            Preview.DrawInline();
        }

        protected abstract void DrawData();

        public void UpdateSkeleton( SimpleSklb sklbFile ) {
            UpdateBones( sklbFile );
            UpdatePreview();
            UpdateData();
        }

        private unsafe void UpdateBones( SimpleSklb sklbFile ) {
            try {
                var tempPath = Path.Combine( Plugin.Configuration.WriteLocation, $"{Extension}_sklb_temp.hkx" );
                sklbFile.SaveHavokData( tempPath.Replace( '\\', '/' ) );

                Bones = new( tempPath );
                Bones.RemoveReference();
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not read file: {Selector.Path}" );
            }
        }

        public static Vector2 CalculateSize( bool skeletonTabOpen, bool splitOpen ) {
            if( skeletonTabOpen ) {
                return new Vector2( -1 );
            }
            else if( splitOpen ) {
                return new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2 );
            }
            return new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing() - ImGui.GetStyle().ItemSpacing.Y * 2f );
        }

        public void DrawSplit( ref bool open ) {
            ImGui.Separator();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( open ? FontAwesomeIcon.AngleDoubleDown.ToIconString() : FontAwesomeIcon.AngleDoubleUp.ToIconString() ) ) {
                    open = !open;
                    Plugin.Configuration.Save();
                }
            }

            if( open ) {
                ImGui.SameLine();
                Draw();
            }
        }

        protected abstract void UpdateData();

        private void UpdatePreview() {
            if( Bones?.BoneList.Count == 0 ) Preview.LoadEmpty( File );
            else Preview.LoadSkeleton( File, Bones.BoneList, new ConnectedSkeletonMeshBuilder( Bones.BoneList ).Build() );
        }

        public void Dispose() {
            if( Preview.CurrentFile == File ) Preview.ClearFile();
        }
    }
}
