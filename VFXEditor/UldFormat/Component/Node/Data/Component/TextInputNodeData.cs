using System;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    [Flags]
    public enum TextInputFlags {
        Capitalize = 0x80,
        Mask = 0x40,
        AutoTranslate = 0x20,
        History = 0x10,
        IME = 0x08,
        EscapeClears = 0x04,
        CapsAllowed = 0x02,
        LowerAllowed = 0x01
    }

    [Flags]
    public enum TextInputFlags2 {
        NumbersAllowed = 0x80,
        SymbolsAllowed = 0x40,
        WordWrap = 0x20,
        Multiline = 0x10,
        AutoMaxWidth = 0x08,
        Unknown = 0x07
    }

    public class TextInputNodeData : UldNodeComponentData {
        public TextInputNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Max Width" ),
                new ParsedUInt( "Max Lines" ),
                new ParsedUInt( "Max Bytes" ),
                new ParsedUInt( "Max Chars" ),
                new ParsedFlag<TextInputFlags>( "Flags", size: 1 ),
                new ParsedFlag<TextInputFlags2>( "Flags 2", size: 1 ),
                new ParsedUInt( "Charset", size: 2 ),
            } );

            for( var i = 1; i <= 16; i++ ) Parsed.Add( new ParsedInt( $"Charset Extra #{i}" ) );
        }
    }
}
