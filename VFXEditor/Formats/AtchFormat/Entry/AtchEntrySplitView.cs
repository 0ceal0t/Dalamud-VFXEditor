using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.AtchFormat.Entry {
    public class AtchEntrySplitView : CommandSplitView<AtchEntry> {
        public AtchEntrySplitView( List<AtchEntry> items ) : base( "Entry", items, false, null, () => new(), () => CommandManager.Atch ) { }

        protected override bool DrawLeftItem( AtchEntry item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            var code = item.Name.Value;
            var weaponName = item.WeaponName;

            ImGui.TextDisabled( code );
            ImGui.SameLine( 35 );
            if( ImGui.Selectable( $"{weaponName}##{code}", item == Selected, ImGuiSelectableFlags.SpanAllColumns ) ) {
                Selected = item;
            }

            return false;
        }
    }
}
