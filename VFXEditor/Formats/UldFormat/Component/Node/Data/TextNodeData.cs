using System;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Color;
using VfxEditor.UldFormat.Widget;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public enum FontType : int {
        Axis = 0x0,
        MiedingerMed = 0x1,
        Miedinger = 0x2,
        TrumpGothic = 0x3,
        Jupiter = 0x4,
        JupiterLarge = 0x5,
    }

    public enum SheetType : int {
        Addon = 0x0,
        Lobby = 0x1,
    }

    [Flags]
    public enum TextFlags {
        Bold = 0x01,
        Italic = 0x02,
        Edge = 0x04,
        Glare = 0x08,
        Multiline = 0x10,
        Ellipsis = 0x20,
        Paragraph = 0x40,
        Emboss = 0x80
    }

    public class TextNodeData : UldGenericData {
        public TextNodeData() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id", size: 2 ),
                new ParsedUInt( "Unknown 1", size: 2 ),
                new ParsedSheetColor( "Color" ),
                new ParsedEnum<AlignmentType>( "Alignment", size: 1 ),
                new ParsedUInt( "Unknown 2", size: 1 ),
                new ParsedEnum<FontType>( "Font", size: 1 ),
                new ParsedInt( "Font Size", size: 1 ),
                new ParsedSheetColor( "Edge Color" ),
                new ParsedFlag<TextFlags>( "Flags", size: 1 ),
                new ParsedEnum<SheetType>( "Sheet Type", size: 1 ),
                new ParsedInt( "Character Spacing", size: 1 ),
                new ParsedInt( "Line Spacing", size: 1 ),
                new ParsedUInt( "Unknown 3", size: 2 ),
                new ParsedUInt( "Unknown 4", size: 2 ),
            ] );
        }
    }
}
