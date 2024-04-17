using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MdlFormat.Vertex {
    public class MdlVertexElement {
        public readonly byte Stream;
        public readonly byte Offset;
        public readonly VertexType Type;
        public readonly VertexUsage Usage;
        public readonly byte UsageIndex;

        public bool NoData => Stream == 255 || Type == 0;

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
            VertexType.Single3 => new( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0 ),
            VertexType.Single4 => new( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() ),
            VertexType.UInt => new( reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() ),
            VertexType.Short2 or VertexType.Short2n => new( reader.ReadInt16(), reader.ReadInt16(), 0, 0 ),
            VertexType.Short4 or VertexType.Short4n => new( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() ),
            VertexType.ByteFloat4 => new Vector4( reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() ) / 255f,
            VertexType.Half2 => new( ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), 0, 0 ),
            VertexType.Half4 => new( ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf(), ( float )reader.ReadHalf() ),
            VertexType.UShort2 => new( reader.ReadUInt16(), reader.ReadUInt16(), 0, 0 ),
            VertexType.UShort4 => new( reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() ),
            _ => new( 0 )
        };

        public void Write( BinaryWriter writer ) {
            writer.Write( Stream );
            writer.Write( Offset );
            writer.Write( ( byte )Type );
            writer.Write( ( byte )Usage );
            writer.Write( UsageIndex );
            FileUtils.Pad( writer, 3 );
        }
    }
}
