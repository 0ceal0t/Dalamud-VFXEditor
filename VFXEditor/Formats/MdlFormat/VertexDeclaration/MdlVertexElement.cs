using System.IO;

namespace VfxEditor.Formats.MdlFormat.VertexDeclaration {
    public class MdlVertexElement {
        public readonly byte Stream;
        public readonly byte Offset;
        public readonly byte Type;
        public readonly byte Usage;
        public readonly byte UsageIndex;

        public bool End => Stream == 255;

        public MdlVertexElement() { }

        public MdlVertexElement( BinaryReader reader ) : this() {
            Stream = reader.ReadByte();
            Offset = reader.ReadByte();
            Type = reader.ReadByte();
            Usage = reader.ReadByte();
            UsageIndex = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding
        }
    }
}
