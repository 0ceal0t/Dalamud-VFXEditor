using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Components;

namespace VfxEditor.ScdFormat.Music {
    public class ScdAudioEntrySplitView : SimpleSplitView<ScdAudioEntry> {
        public ScdAudioEntrySplitView( List<ScdAudioEntry> items ) : base( "Audio", items, false, false ) { }

        protected override bool DrawLeftItem( ScdAudioEntry item, int idx) {
            if( item.DataLength == 0 ) return false;
            using var _ = ImRaii.PushId( idx );
            if( ImGui.Selectable( $"{ItemName} {idx}", item == Selected ) ) Selected = item;
            return false;
        }
    }
}
