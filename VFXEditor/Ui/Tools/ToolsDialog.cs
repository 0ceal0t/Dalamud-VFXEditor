using ImGuiNET;
using OtterGui.Raii;

namespace VfxEditor.Ui.Tools {
    public partial class ToolsDialog : DalamudWindow {
        private readonly ResourceTab ResourceTab;
        private readonly UtilitiesTab UtilitiesTab;
        private readonly LoadedTab LoadedTab;
        private readonly LuaTab LuaTab;

        public ToolsDialog() : base( "Tools", false, new( 300, 400 ) ) {
            ResourceTab = new ResourceTab();
            UtilitiesTab = new UtilitiesTab();
            LoadedTab = new LoadedTab();
            LuaTab = new LuaTab();
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
            if( ImGui.BeginTabItem( "Lua Variables" ) ) {
                LuaTab.Draw();
                ImGui.EndTabItem();
            }
        }
    }
}