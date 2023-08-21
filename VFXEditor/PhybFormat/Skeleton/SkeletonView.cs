using Dalamud.Interface;
using Dalamud.Logging;
using HelixToolkit.SharpDX.Core;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using VfxEditor.DirectX;
using VfxEditor.Interop.Havok;
using VfxEditor.Utils;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.PhybFormat.Skeleton {
    public class SkeletonView {
        private readonly PhybFile File;
        private HavokBones Bones;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";

        private static BoneNamePreview PhybPreview => Plugin.DirectXManager.PhybPreview;

        public SkeletonView( PhybFile file, string sourcePath ) {
            File = file;

            if( string.IsNullOrEmpty( sourcePath ) ) return;
            var sklbPath = sourcePath.Replace( ".phyb", ".sklb" ).Replace( "phy_", "skl_" );
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;
        }

        public void Draw() {
            if( Bones == null ) LoadSklbPath();
            else if( PhybPreview.CurrentFile != File ) {
                UpdateSkeleton();
                UpdatePhysicsObjects();
            }

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
            var inputSize = UiUtils.GetOffsetInputSize( checkSize + 200 );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vec2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) LoadSklbPath();

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileDialogManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        UpdateBones( SimpleSklb.LoadFromLocal( res ) );
                        UpdateSkeleton();
                        UpdatePhysicsObjects();
                    } );
                }
            }

            ImGui.SameLine();

            if( ImGui.Checkbox( "Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();

            if( Bones == null ) return;
            if( File.PhysicsUpdated ) UpdatePhysicsObjects();
            PhybPreview.DrawInline();
        }

        private void LoadSklbPath() {
            if( Plugin.DataManager.FileExists( SklbPreviewPath ) ) {
                UpdateBones( Plugin.DataManager.GetFile<SimpleSklb>( SklbPreviewPath ) );
                UpdateSkeleton();
                UpdatePhysicsObjects();
            }
            else {
                PluginLog.Error( $"File does not exist: {SklbPreviewPath}" );
            }
        }

        private unsafe void UpdateBones( SimpleSklb sklbFile ) {
            try {
                var tempPath = Path.Combine( Plugin.Configuration.WriteLocation, $"phyb_skl_temp.hkx" );
                sklbFile.SaveHavokData( tempPath.Replace( '\\', '/' ) );

                Bones = new( tempPath );
                Bones.RemoveReference();
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not read file: {SklbPreviewPath}" );
            }
        }

        private void UpdatePhysicsObjects() {
            File.PhysicsUpdated = false;
            if( Bones?.BoneList.Count == 0 ) return;

            MeshBuilders meshes = new() {
                Collision = new MeshBuilder( true, false ),
                Simulation = new MeshBuilder( true, false ),
                Spring = new MeshBuilder( true, false )
            };

            File.AddPhysicsObjects( meshes, Bones.BoneMatrixes );
            PhybPreview.LoadWireframe( meshes.Collision.ToMesh(), meshes.Simulation.ToMesh(), meshes.Spring.ToMesh() );
        }

        private void UpdateSkeleton() {
            if( Bones?.BoneList.Count == 0 ) PhybPreview.LoadEmpty( File );
            else PhybPreview.LoadSkeleton( File, Bones.BoneList, AnimationData.CreateSkeletonMesh( Bones.BoneList, -1 ) );
        }

        public void Dispose() {
            if( PhybPreview.CurrentFile == File ) PhybPreview.ClearFile();
        }
    }
}
