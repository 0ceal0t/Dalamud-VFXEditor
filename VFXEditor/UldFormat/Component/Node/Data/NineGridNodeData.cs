using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public enum GridPartsType : int {
        Divide = 0x0,
        Compose = 0x1,
    }

    public enum GridRenderType : int {
        Scale = 0x0,
        Tile = 0x1,
    }

    public class NineGridNodeData : UldGenericData {
        public NineGridNodeData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Part List Id", size: 2 ),
                new ParsedUInt( "Unknown 1", size: 2 ),
                new ParsedUInt( "Part Id" ),
                new ParsedEnum<GridPartsType>( "Grid Parts Type", size: 1 ),
                new ParsedEnum<GridRenderType>( "Grid Render Type", size: 1 ),
                new ParsedShort( "Top Offset" ),
                new ParsedShort( "Bottom Offset" ),
                new ParsedShort( "Left Offset" ),
                new ParsedShort( "Right Offset" ),
                new ParsedInt( "Unknown 2", size: 1 ),
                new ParsedInt( "Unknown 3", size: 1 ),
                new ParsedInt( "Unknown 4", size: 1 ),
            } );
        }
    }
}
