using Dalamud.Interface;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;

namespace VfxEditor.FileBrowser.SideBar {
    public class FileBrowserSidebarItem {
        public FontAwesomeIcon Icon;
        public string Text;
        public string Location;

        public bool Draw( bool selected ) {
            var ret = false;

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Selectable( Icon.ToIconString(), selected ) ) ret = true;
            }
            ImGui.SameLine( 25 );
            ImGui.Text( Text );

            return ret;
        }
    }
}
