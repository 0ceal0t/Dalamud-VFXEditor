using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using VFXEditor.Utils;
using VFXEditor.Texture;

namespace VFXEditor.Dialogs {
    public class ToolsDialogUtilitiesTab {
        private string RawInputValue = "";
        private string RawTexInputValue = "";

        public void Draw() {
            ImGui.Text( "Extract a raw .avfx file" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) ) {
                var result = VfxEditor.DataManager.FileExists( RawInputValue );
                if( result ) {
                    try {
                        var file = VfxEditor.DataManager.GetFile( RawInputValue );
                        UiUtils.WriteBytesDialog( ".avfx", file.Data, "avfx" );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( "Could not read file", e );
                    }
                }
            }

            ImGui.Text( "Extract an .atex file" );
            ImGui.InputText( "Path##RawTexExtract", ref RawTexInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawTexExtract" ) ) {
                var result = VfxEditor.DataManager.FileExists( RawTexInputValue );
                if( result ) {
                    try {
                        var file = VfxEditor.DataManager.GetFile( RawTexInputValue );
                        UiUtils.WriteBytesDialog( ".atex", file.Data, "atex" );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( e, "Could not read file" );
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
