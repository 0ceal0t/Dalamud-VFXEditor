using System;
using System.Numerics;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI {
    public class ToolsDialog : GenericDialog {
        private string RawInputValue = "";
        private string RawTexInputValue = "";

        public ToolsDialog( Plugin plugin ) : base( plugin, "Tools" ) {
            Size = new Vector2( 300, 150 );
        }

        public override void OnDraw() {
            // ======= AVFX =========
            ImGui.Text( "Extract a raw .avfx file" );
            ImGui.InputText( "Path##RawExtract", ref RawInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawExtract" ) ) {
                bool result = Plugin.PluginInterface.Data.FileExists( RawInputValue );
                if( result ) {
                    try {
                        var file = Plugin.PluginInterface.Data.GetFile( RawInputValue );
                        Plugin.WriteBytesDialog( ".avfx", file.Data, "avfx" );
                    }
                    catch( Exception e ) {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }
            // ===== ATEX ==========
            ImGui.Text( "Extract an .atex file" );
            ImGui.InputText( "Path##RawTexExtract", ref RawTexInputValue, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Extract##RawTexExtract" ) ) {
                bool result = Plugin.PluginInterface.Data.FileExists( RawTexInputValue );
                if( result ) {
                    try {
                        var file = Plugin.PluginInterface.Data.GetFile( RawTexInputValue );
                        Plugin.WriteBytesDialog( ".atex", file.Data, "atex" );
                    }
                    catch( Exception e ) {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }

            ImGui.Text( ".atex to PNG" );
            ImGui.SameLine();
            if(ImGui.Button("Browse##AtexToPNG")) {
                Plugin.DialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) =>
                {
                    if( !ok ) return;
                    var texFile = VFXTexture.LoadFromLocal( res );
                    texFile.SaveAsPng( res + ".png" );
                } );
            }

            ImGui.Text( ".atex to DDS" );
            ImGui.SameLine();
            if( ImGui.Button( "Browse##AtexToDDS" ) ) {
                Plugin.DialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) =>
                {
                    if( !ok ) return;
                    var texFile = VFXTexture.LoadFromLocal( res );
                    texFile.SaveAsDDS( res + ".dds" );
                } );
            }
        }
    }
}