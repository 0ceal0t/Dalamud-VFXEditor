using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Numerics;
using VFXEditor.Helper;

namespace VFXEditor.Texture {
    public partial class TextureManager {
        private string newCustomPath = string.Empty;

        public override void DrawBody() {
            var id = "##ImportTex";

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild( id + "/Child", new Vector2( 0, -( footerHeight + ImGui.GetStyle().ItemSpacing.Y ) ), true );

            if( PathToTextureReplace.IsEmpty ) {
                ImGui.Text( "No textures have been imported..." );
            }

            var idx = 0;
            foreach( var entry in PathToTextureReplace ) {
                if( ImGui.CollapsingHeader( $"{entry.Key}##{id}-{idx}" ) ) {
                    ImGui.Indent();
                    DrawTexture( entry.Key + '\u0000', $"{id}-{idx}" );
                    ImGui.Unindent();
                }
                idx++;
            }

            ImGui.EndChild();


            ImGui.SetNextItemWidth( ImGui.GetWindowContentRegionWidth() - 150 );
            ImGui.InputText( $"{id}-Input", ref newCustomPath, 255 );

            ImGui.SameLine();
            if( ImGui.Button( $"Add Custom Texture##{id}" ) ) {
                var path = newCustomPath.Trim().Trim( '\0' ).ToLower();
                if (!string.IsNullOrEmpty(path) && !PathToTextureReplace.ContainsKey(path)) {
                    ImportDialog( path );
                }
            }
        }

        public void DrawTexture( string path, string id ) {
            if( GetTexturePreview( path, out var texOut ) ) {
                ImGui.Image( texOut.Wrap.ImGuiHandle, new Vector2( texOut.Width, texOut.Height ) );
                ImGui.Text( $"Format: {texOut.Format}  MIPS: {texOut.MipLevels}  SIZE: {texOut.Width}x{texOut.Height}" );
                if( ImGui.Button( "Export" + id ) ) {
                    ImGui.OpenPopup( "Tex_Export" + id );
                }
                ImGui.SameLine();
                if( ImGui.Button( "Replace" + id ) ) {
                    ImportDialog( path.Trim( '\0' ) );
                }
                if( ImGui.BeginPopup( "Tex_Export" + id ) ) {
                    if( ImGui.Selectable( "PNG" + id ) ) {
                        SavePngDialog( path.Trim( '\0' ) );
                    }
                    if( ImGui.Selectable( "DDS" + id ) ) {
                        SaveDDSDialog( path.Trim( '\0' ) );
                    }
                    ImGui.EndPopup();
                }

                // Imported texture
                if( texOut.IsReplaced ) {
                    ImGui.SameLine();
                    if( UIHelper.RemoveButton( "Remove Replaced Texture" + id, small: true ) ) {
                        RemoveReplaceTexture( path.Trim( '\0' ) );
                        RefreshPreviewTexture( path.Trim( '\0' ) );
                    }
                }
            }
        }

        private void ImportDialog( string newPath ) {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( !AddReplaceTexture( res, newPath ) ) {
                        PluginLog.Error( $"Could not import" );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        private void SavePngDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsPNG( res );
            } );
        }

        private void SaveDDSDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = GetRawTexture( texPath );
                texFile.SaveAsDDS( res );
            } );
        }
    }
}
