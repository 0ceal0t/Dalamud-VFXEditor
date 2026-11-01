using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.ScdFormat.Music {
    public class ScdAudioEntrySplitView : CommandSplitView<ScdAudioEntry> {
        public ScdAudioEntrySplitView( ScdFile file, List<ScdAudioEntry> items ) : base( "Audio", items, false, null, () => ScdAudioEntry.Default( file ) ) { }

        protected override bool DrawLeftItem( ScdAudioEntry item, int idx ) {
            if( item.DataLength == 0 ) return false;
            using var _ = ImRaii.PushId( idx );
            if( ImGui.Selectable( $"{Id} {idx}", item == Selected ) ) Selected = item;
            return false;
        }
    }
}
