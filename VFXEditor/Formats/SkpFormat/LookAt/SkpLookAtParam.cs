using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat.LookAt {
    public class SkpLookAtParam : ParsedData, IUiItem, ITextItem {
        public readonly ParsedByte Index = new( "Index" );

        public SkpLookAtParam() : base() { }

        public SkpLookAtParam( BinaryReader reader ) : base() {
            ReadParsed( reader );
            reader.ReadBytes( 3 );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            new ParsedFloat4( "Limit Angles" ),
            new ParsedFloat3( "Forward Rotation" ),
            new ParsedFloat( "Limit Angle" ),
            new ParsedFloat3( "Eye Positions" ),
            new ParsedUInt( "Flags" ),
            new ParsedFloat( "Gain" ),
            Index,
        };

        public void Write( BinaryWriter writer ) {
            WriteParsed( writer );
            FileUtils.Pad( writer, 3 );
        }

        public void Draw() {
            // Draw the index first
            Index.Draw( CommandManager.Skp );
            foreach( var item in Parsed.Where( x => x != Index ) ) item.Draw( CommandManager.Skp );
        }

        public string GetText() => $"Parameters {Index.Value}";
    }
}
