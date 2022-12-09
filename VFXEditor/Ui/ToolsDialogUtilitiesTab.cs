using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using VfxEditor.Utils;
using VfxEditor.Texture;

namespace VfxEditor.Ui {
    public class ToolsDialogUtilitiesTab {
        private string ExtractPath = "";

        public void Draw() {
            ImGui.Text( "Extract a raw game file" );
            ImGui.InputText( "Path##RawExtract", ref ExtractPath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) ) {
                var cleanedPath = ExtractPath.Replace( "\\", "/" );
                if( Plugin.DataManager.FileExists( cleanedPath ) ) {
                    try {
                        var ext = cleanedPath.Contains( '.' ) ? cleanedPath.Split( "." )[1] : "bin";
                        var file = Plugin.DataManager.GetFile( cleanedPath );
                        UiUtils.WriteBytesDialog( $".{ext}", file.Data, ext );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( "Could not read file", e );
                    }
                }
            }

            ImGui.Text( ".atex to PNG" );
            ImGui.SameLine();
            if( ImGui.Button( "Browse##AtexToPNG" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = AtexFile.LoadFromLocal( res );
                    texFile.SaveAsPng( res + ".png" );
                } );
            }

            ImGui.Text( ".atex to DDS" );
            ImGui.SameLine();
            if( ImGui.Button( "Browse##AtexToDDS" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = AtexFile.LoadFromLocal( res );
                    texFile.SaveAsDds( res + ".dds" );
                } );
            }
        }
    }
}
