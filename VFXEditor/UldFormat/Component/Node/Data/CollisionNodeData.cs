using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public enum CollisionType : int {
        Hit = 0x0,
        Focus = 0x1,
        Move = 0x2,
    }

    public class CollisionNodeData : UldNodeData {
        public CollisionNodeData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedEnum<CollisionType>( "Collision Type", size: 2 ),
                new ParsedUInt( "Unknown 1", size: 2 ),
                new ParsedInt( "X" ),
                new ParsedInt( "Y" ),
                new ParsedUInt( "Radius" ),
            } );
        }
    }
}
