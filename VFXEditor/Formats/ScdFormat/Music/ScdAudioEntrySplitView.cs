using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Data.Command;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.ScdFormat.Music {
    public class ScdAudioEntrySplitView : CommandSplitView<ScdAudioEntry> {
        private readonly ScdFile File;

        public ScdAudioEntrySplitView( ScdFile file, List<ScdAudioEntry> items ) : base( "Audio", items, false, null, () => ScdAudioEntry.Default( file ) ) {
            File = file;
        }

        protected override void OnDelete( ScdAudioEntry item ) => CommandManager.Add( new ScdAudioRemoveCommand( File, item ) );

        public void SwapSelected( ScdAudioEntry from, ScdAudioEntry to ) {
            if( Selected == from ) Selected = to;
        }

        protected override bool DrawLeftItem( ScdAudioEntry item, int idx ) {
            if( item.DataLength == 0 ) return false;
            using var _ = ImRaii.PushId( idx );
            if( ImGui.Selectable( $"{Id} {idx}", item == Selected ) ) Selected = item;
            return false;
        }
    }
}