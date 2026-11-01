using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.ScdFormat.Layout {
    public class ScdLayoutData : IData {
        public List<ParsedBase> Parsed;

        public void Read( BinaryReader reader ) => Parsed.ForEach( x => x.Read( reader ) );

        public void Write( BinaryWriter writer ) => Parsed.ForEach( x => x.Write( writer ) );

        public void Draw() => Parsed.ForEach( x => x.Draw() );

        public void Enable() { }

        public void Disable() { }
    }
}
