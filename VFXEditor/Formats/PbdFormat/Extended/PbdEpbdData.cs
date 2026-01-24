using System.Collections.Generic;
using System.Linq;
using VfxEditor.Flatbuffer;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.PbdFormat.Extended {
    public class PbdEpbdData {
        private readonly EpbdUnknown Unknown1;
        private readonly EpbdUnknown Unknown2;

        public readonly List<PbdEpbdEntry> Entries = [];
        private readonly CommandDropdown<PbdEpbdEntry> EntryView;

        public PbdEpbdData( ExtendedPbd table ) {
            Unknown1 = table.Epbd1;
            Unknown2 = table.Epbd2;
            Entries.AddRange( table.Pbdist.Select( x => new PbdEpbdEntry( x ) ) );

            EntryView = new( "Entries", Entries, null, () => new() );
        }

        public void Draw() => EntryView.Draw();

        public ExtendedPbd Export() => new() {
            Epbd1 = Unknown1,
            Epbd2 = Unknown2,
            Pbdist = [.. Entries.Select( x => x.Export() )],
        };
    }
}
