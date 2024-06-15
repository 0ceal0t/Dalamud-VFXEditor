using System.Collections.Generic;
using System.Linq;
using VfxEditor.Ui.Components;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbTable {
        public readonly EphbUnknownTT Unknown1;
        public readonly EphbUnknownTT Unknown2;

        public readonly List<PhybEphbAlpha> Alpha = [];
        private readonly CommandDropdown<PhybEphbAlpha> AlphaView;

        public PhybEphbTable( EphbTableT table ) {
            Unknown1 = table.Unknown1;
            Unknown2 = table.Unknown2;
            Alpha.AddRange( table.Alpha.Select( x => new PhybEphbAlpha( x ) ) );

            AlphaView = new( "Alpha", Alpha, null, () => new() );
        }

        public void Draw() => AlphaView.Draw();

        public EphbTableT Export() => new() {
            Unknown1 = Unknown1,
            Unknown2 = Unknown2,
            Alpha = Alpha.Select( x => x.Export() ).ToList()
        };
    }
}
