using System.Collections.Generic;
using System.Linq;
using VfxEditor.Flatbuffer;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbTable {
        public readonly EphbUnknownT Unknown1;
        public readonly EphbUnknownT Unknown2;

        public readonly List<PhybEphbAlpha> Alpha = [];
        private readonly CommandDropdown<PhybEphbAlpha> AlphaView;

        public PhybEphbTable( EphbData table ) {
            Unknown1 = table.Unknown1;
            Unknown2 = table.Unknown2;
            Alpha.AddRange( table.Alpha.Select( x => new PhybEphbAlpha( x ) ) );

            AlphaView = new( "Alpha", Alpha, null, () => new() );
        }

        public void Draw() => AlphaView.Draw();

        public EphbData Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Alpha = Alpha.Select( x => x.Export() ).ToList()
        };
    }
}
