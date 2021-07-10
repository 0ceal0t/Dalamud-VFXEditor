using System;
using System.Numerics;
using ImGuiNET;

namespace VFXEditor.UI {
    public class SettingsDialog : GenericDialog {
        public SettingsDialog( Plugin plugin ) : base( plugin, "Settings" ) {
            Size = new Vector2( 300, 100 );
        }

        public override void OnDraw() {
            ImGui.Text( "Changes to the temp file location may require a restart to take effect" );
            if(ImGui.InputText( "Temp file location", ref Configuration.Config.WriteLocation, 255 )) {
                Configuration.Config.Save();
            }

            ImGui.SetNextItemWidth( 200 );
            if(ImGui.Checkbox( "Verify on load##Settings", ref Configuration.Config.VerifyOnLoad )) {
                Configuration.Config.Save();
            }

            if(ImGui.Checkbox( "Log all files##Settings", ref Configuration.Config.LogAllFiles )) {
                Configuration.Config.Save();
            }

            if(ImGui.Checkbox( "Hide with UI##Settings", ref Configuration.Config.HideWithUI )) {
                Configuration.Config.Save();
            }

            ImGui.SetNextItemWidth( 135 );
            if( ImGui.InputInt( "Recent VFX Limit##Settings", ref Configuration.Config.SaveRecentLimit ) ) {
                Configuration.Config.SaveRecentLimit = Math.Max( Configuration.Config.SaveRecentLimit, 0 );
                Configuration.Config.Save();
            }

            if(ImGui.Checkbox( "Live Overlay Limit by Distance##Settings", ref Configuration.Config.OverlayLimit )) {
                Configuration.Config.Save();
            }
        }
    }
}