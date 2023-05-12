using ImGuiNET;
using OtterGui.Raii;

namespace VfxEditor.Ui {
    public partial class ToolsDialog : GenericDialog {
        private readonly ToolsDialogResourceTab ResourceTab;
        private readonly ToolsDialogUtilitiesTab UtilitiesTab;

        public ToolsDialog() : base( "Tools", false, 300, 400 ) {
            ResourceTab = new ToolsDialogResourceTab();
            UtilitiesTab = new ToolsDialogUtilitiesTab();
        }

        public override void DrawBody() {
            using var id = ImRaii.PushId( "ToolsTab" );
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( tabBar ) return;

            if( ImGui.BeginTabItem( "Resources" ) ) {
                ResourceTab.Draw();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Utilities" ) ) {
                UtilitiesTab.Draw();
                ImGui.EndTabItem();
            }
        }
    }
}