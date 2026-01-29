using System.Collections.Generic;
using System.Linq;
using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.PhybFormat.Extended.Ephb {
    public class PhybEphbData {
        public readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedUInt Unknown2 = new( "Unknown 2" );

        public readonly List<PhybEphbAlpha> Alpha = [];
        private readonly CommandDropdown<PhybEphbAlpha> AlphaView;

        public PhybEphbData( EphbData table ) {
            Unknown1.Value = table.Unknown1;
            Unknown2.Value = table.Unknown2;
            Alpha.AddRange( table.Alpha.Select( x => new PhybEphbAlpha( x ) ) );

            AlphaView = new( "Alpha", Alpha, null, () => new() );
        }

        public void Draw() => AlphaView.Draw();

        public EphbData Export() => new() {
            Unknown1 = Unknown1.Value,
            Unknown2 = Unknown2.Value,
            Alpha = [.. Alpha.Select( x => x.Export() )]
        };
    }
}
