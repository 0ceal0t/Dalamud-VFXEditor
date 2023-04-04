using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldMainHeader : UldGenericHeader {
        public uint ComponentOffset;
        public uint WidgetOffset;

        public UldMainHeader( BinaryReader reader ) : base( reader ) {
            ComponentOffset = reader.ReadUInt32();
            WidgetOffset = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer, out long offsetsPosition ) {
            WriteHeader( writer );
            offsetsPosition = writer.BaseStream.Position;
            writer.Write( ComponentOffset );
            writer.Write( WidgetOffset );
        }
    }
}
