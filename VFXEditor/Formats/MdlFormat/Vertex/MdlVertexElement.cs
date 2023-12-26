using System.IO;

namespace VfxEditor.Formats.MdlFormat.Vertex {
    public class MdlVertexElement {
        public readonly byte Stream;
        public readonly byte Offset;
        public readonly VertexType Type;
        public readonly VertexUsage Usage;
        public readonly byte UsageIndex;

        public bool End => Stream == 255;

        public int Size => Type switch {
            VertexType.Single3 => 12,
            VertexType.Single4 => 16,
            VertexType.UInt => 4,
            VertexType.ByteFloat4 => 4,
            VertexType.Half2 => 4,
            VertexType.Half4 => 8,
            _ => 0
        };

        public MdlVertexElement() { }

        public MdlVertexElement( BinaryReader reader ) : this() {
            Stream = reader.ReadByte();
            Offset = reader.ReadByte();
            Type = ( VertexType )reader.ReadByte();
            Usage = ( VertexUsage )reader.ReadByte();
            UsageIndex = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding

            if( !End ) {
                Dalamud.Log( $"Element: {Stream} {Offset:X4} {Type} {Usage}" );
            }
        }
    }
}
