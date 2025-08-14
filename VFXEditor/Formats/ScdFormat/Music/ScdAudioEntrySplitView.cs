using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.ScdFormat.Music {
    public class ScdAudioEntrySplitView : CommandSplitView<ScdAudioEntry> {
        public ScdAudioEntrySplitView( List<ScdAudioEntry> items ) : base( "Audio", items, false, null, () => ScdAudioEntry.Default( Plugin.ScdManager.File ) ) { }

        protected override bool DrawLeftItem( ScdAudioEntry item, int idx ) {
            if( item.DataLength == 0 ) return false;
            using var _ = ImRaii.PushId( idx );
            if( ImGui.Selectable( $"{Id} {idx}", item == Selected ) ) Selected = item;
            return false;
        }
    }
}
