using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.DirectX;
using VfxEditor.FileManager;
using VfxEditor.Interop.Havok.SkeletonBuilder;
using VfxEditor.Utils;
using VfxEditor.DirectX.Bone;

namespace VfxEditor.Interop.Havok.Ui {
    public abstract class SkeletonView {
        public readonly int RenderId = RenderInstance.NewId;
        protected readonly BoneNameInstance Instance;

        private readonly FileManagerFile File;
        private readonly string Extension;

        protected HavokBones Bones;
        protected readonly SkeletonSelector Selector;

        public SkeletonView( FileManagerFile file, BoneNameInstance instance, string sklbPath, string extension ) {
            File = file;
            Instance = instance;
            Extension = extension;
            if( !string.IsNullOrEmpty( sklbPath ) ) sklbPath = sklbPath.Replace( $".{extension}", ".sklb" ).Replace( $"{extension[0..3]}_", "skl_" );
            Selector = new( sklbPath, UpdateSkeleton );
        }

        public void Draw() {
            Selector.Draw();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
                if( ImGui.Checkbox( "Show Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();
            }

            if( Bones == null ) return;
            DrawData();
            Plugin.DirectXManager.BoneNameRenderer.DrawTexture( RenderId, Instance, UpdateRender, Plugin.Configuration.DrawDirectXSkeleton );
        }

        protected abstract void DrawData();

        public void UpdateRender() {
            UpdatePreview();
            UpdateData();
        }

        public void UpdateSkeleton( SimpleSklb sklbFile ) {
            UpdateBones( sklbFile );
            Plugin.DirectXManager.BoneNameRenderer.NeedsUpdate = true;
        }

        private unsafe void UpdateBones( SimpleSklb sklbFile ) {
            try {
                var tempPath = Path.Combine( Plugin.Configuration.WriteLocation, $"{Extension}_sklb_temp.hkx" );
                sklbFile.SaveHavokData( tempPath.Replace( '\\', '/' ) );

                Bones = new( tempPath, true );
                Bones.RemoveReference();
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not read file: {Selector.Path}" );
            }
        }

        public static Vector2 CalculateSize( bool skeletonTabOpen, bool splitOpen ) {
            if( skeletonTabOpen ) {
                return new Vector2( -1 );
            }
            else if( splitOpen ) {
                return new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2 );
            }
            return new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - UiUtils.AngleUpDownSize );
        }

        public void DrawSplit( ref bool open ) {
            if( UiUtils.DrawAngleUpDown( ref open ) ) Plugin.Configuration.Save();
            if( open ) Draw();
        }

        protected abstract void UpdateData();

        private void UpdatePreview() {
            if( Bones?.BoneList.Count == 0 ) Plugin.DirectXManager.BoneNameRenderer.SetEmpty( RenderId, Instance, File );
            else Plugin.DirectXManager.BoneNameRenderer.SetSkeleton( RenderId, Instance, File, Bones.BoneList, new ConnectedSkeletonMeshBuilder( Bones.BoneList ).Build() );
        }
    }
}
