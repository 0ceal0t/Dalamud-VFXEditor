using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Interop.Penumbra.Meta;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.AtchFormat.Entry {
    public class AtchEntrySplitView : CommandSplitView<AtchEntry> {
        private readonly AtchFile File;

        public AtchEntrySplitView( AtchFile file, List<AtchEntry> items ) : base( "Entry", items, false, null, () => new() ) {
            File = file;
        }

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

        protected override void DrawSelected() {
            using var _ = ImRaii.PushId( Items.IndexOf( Selected ) );
            var states = PenumbraMetaWriter
                .GetUpdatedStates( File.Vanilla?.Entries, Selected, File.SeenNames, 0 )
                .Select( x => x.State )
                .ToList();
            Selected.Draw( states );
        }
    }
}