using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace VfxEditor.FileBrowser.SideBar {
    public class FileBrowserSidebarItem {
        public FontAwesomeIcon Icon;
        public string Text;
        public string Location;

        public bool Draw( bool selected ) {
            var ret = false;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Selectable( Icon.ToIconString(), selected ) ) ret = true;
            }
            ImGui.SameLine();
            ImGui.Text( Text );

            return ret;
        }
    }
}
