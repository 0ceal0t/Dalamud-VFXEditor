using System;
using System.Numerics;
using ImGuiNET;

namespace VFXEditor.Dialogs {
    public partial class ToolsDialog : GenericDialog {
        public ToolsDialog() : base( "Tools" ) {
            Size = new Vector2( 300, 400 );
        }

        public override void DrawBody() {
            if( ImGui.BeginTabBar( "##ToolsTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Resources##ToolsTabs" ) ) {
                    DrawResources();
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Utilities##ToolsTabs" ) ) {
                    DrawUtilities();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
    }
}