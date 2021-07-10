using System;
using System.Numerics;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {
    public class ToolsDialog : GenericDialog {
        private string RawInputValue = "";
        private string RawTexInputValue = "";

        public ToolsDialog( Plugin plugin ) : base( plugin, "Tools" ) {
            Size = new Vector2( 300, 100 );
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
                        Plugin.SaveDialog( "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*", file.Data, "avfx" );
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
                        Plugin.SaveDialog( "ATEX File (*.atex)|*.atex*|All files (*.*)|*.*", file.Data, "atex" );
                    }
                    catch( Exception e ) {
                        PluginLog.LogError( "Could not read file" );
                        PluginLog.LogError( e.ToString() );
                    }
                }
            }
        }
    }
}