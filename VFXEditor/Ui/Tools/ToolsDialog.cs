using ImGuiNET;
using OtterGui.Raii;

namespace VfxEditor.Ui.Tools {
    public partial class ToolsDialog : GenericDialog {
        private readonly ResourceTab ResourceTab;
        private readonly UtilitiesTab UtilitiesTab;
        private readonly LoadedTab LoadedTab;

        public ToolsDialog() : base( "Tools", false, 300, 400 ) {
            ResourceTab = new ResourceTab();
            UtilitiesTab = new UtilitiesTab();
            LoadedTab = new LoadedTab();
        }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "ToolsTab" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Resources" ) ) {
                ResourceTab.Draw();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Utilities" ) ) {
                UtilitiesTab.Draw();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Loaded Files" ) ) {
                LoadedTab.Draw();
                ImGui.EndTabItem();
            }
        }
    }
}