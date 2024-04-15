using System.IO;
using System.Numerics;

namespace VfxEditor.Formats.MdlFormat.Vertex {
    public class MdlVertexElement {
        public readonly byte Stream;
        public readonly byte Offset;
        public readonly VertexType Type;
        public readonly VertexUsage Usage;
        public readonly byte UsageIndex;

        public bool NoData => Stream == 255 || Type == 0;
        public int EndOffset => Offset + Size;

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
        }

        public Vector4 Read( BinaryReader reader ) => Type switch {
            VertexType.Half2 => new( ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), 0, 0 ),
            VertexType.Half4 => new( ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf() ),
            VertexType.Single3 => new( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0 ),
            VertexType.Single4 => new( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() ),
            VertexType.UInt or VertexType.ByteFloat4 => new( reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() ),
            _ => new( 0 )
        };

        public void Write( BinaryWriter writer ) {
            writer.Write( Stream );
            writer.Write( Offset );
            writer.Write( ( byte )Type );
            writer.Write( ( byte )Usage );
            writer.Write( UsageIndex );
            for( var i = 0; i < 3; i++ ) writer.Write( ( byte )0 ); // padding
        }
    }
}
