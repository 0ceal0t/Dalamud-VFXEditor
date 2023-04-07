using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class NineGridTextComponentData : UldComponentData {
        public NineGridTextComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
            } );
        }
    }
}
