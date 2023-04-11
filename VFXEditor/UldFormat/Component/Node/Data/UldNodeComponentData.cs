using System;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    [Flags]
    public enum NodeComponentFlags {
        RepeatUp = 0x80,
        RepeatDown = 0x40,
        RepeatLeft = 0x20,
        RepeatRight = 0x10,
        UnknownFlag = 0x0F
    }

    public class UldNodeComponentData : UldGenericData {
        public UldNodeComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedInt( "Index", size: 1 ),
                new ParsedInt( "Up", size: 1 ),
                new ParsedInt( "Down", size: 1 ),
                new ParsedInt( "Left", size: 1 ),
                new ParsedInt( "Right", size: 1 ),
                new ParsedInt( "Cursor", size: 1 ),
                new ParsedFlag<NodeComponentFlags>( "Flags", size: 1 ),
                new ParsedInt( "Unknown", size: 1 ),
                new ParsedShort( "Offset X" ),
                new ParsedShort( "Offset Y" )
            } );
        }
    }
}
