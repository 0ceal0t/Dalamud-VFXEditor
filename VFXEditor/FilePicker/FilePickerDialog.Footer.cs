using ImGuiNET;
using OtterGui.Raii;
using VfxEditor.Utils;

namespace VfxEditor.FilePicker {
    public partial class FilePickerDialog {
        private bool DrawFooter() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() - ImGui.GetStyle().ItemSpacing.Y );
            ImGui.Separator();

            ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X - 100 - ( Filters.Filters.Count == 0 ? 0 : 150 ) );
            using( var disabled = ImRaii.Disabled( SelectOnly ) ) {
                ImGui.InputText( "##FileName", ref FileNameInput, 255, SelectOnly ? ImGuiInputTextFlags.ReadOnly : ImGuiInputTextFlags.None );
            }

            Filters.Draw();

            var res = false;

            using( var disabled = ImRaii.Disabled( string.IsNullOrEmpty( FileNameInput ) || ( SelectOnly && Selected == null ) ) ) {
                ImGui.SameLine();
                if( ImGui.Button( "OK" ) ) {
                    IsOk = true;
                    res = true;
                }
            }

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Cancel" ) ) {
                IsOk = false;
                res = true;
            }

            if( WantsToQuit && IsOk ) return true;

            return res;
        }
    }
}
