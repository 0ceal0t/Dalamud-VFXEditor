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
using VfxEditor.Interop.Havok.SkeletonBuilder;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat.Skeleton {
    public class SkeletonView {
        private readonly PhybFile File;
        private HavokBones Bones;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";
        private bool SklbReplaced = false;

        private static BoneNamePreview PhybPreview => Plugin.DirectXManager.PhybPreview;

        public SkeletonView( PhybFile file, string sourcePath ) {
            File = file;

            if( string.IsNullOrEmpty( sourcePath ) ) return;
            var sklbPath = sourcePath.Replace( ".phyb", ".sklb" ).Replace( "phy_", "skl_" );
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;
        }

        public void Draw() {
            if( Bones == null ) {
                Plugin.SklbManager.GetSimpleSklb( SklbPreviewPath, out var simple, out var replaced );
                SklbReplaced = replaced;
                UpdateSkeleton( simple );
            }
            else if( PhybPreview.CurrentFile != File ) {
                UpdatePreview();
                UpdatePhysicsObjects();
            }

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Sync );
            var inputSize = ImGui.GetContentRegionAvail().X - 400;
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
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

                ImGui.SameLine();
                if( ImGui.Checkbox( "Show Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();
            }

            if( Bones == null ) return;
            if( File.PhysicsUpdated ) UpdatePhysicsObjects();
            PhybPreview.DrawInline();
        }

        private void UpdateSkeleton( SimpleSklb sklbFile ) {
            UpdateBones( sklbFile );
            UpdatePreview();
            UpdatePhysicsObjects();
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

        private void UpdatePreview() {
            if( Bones?.BoneList.Count == 0 ) PhybPreview.LoadEmpty( File );
            else PhybPreview.LoadSkeleton( File, Bones.BoneList, new ConnectedSkeletonMeshBuilder( Bones.BoneList, -1 ).Build() );
        }

        public void Dispose() {
            if( PhybPreview.CurrentFile == File ) PhybPreview.ClearFile();
        }
    }
}
