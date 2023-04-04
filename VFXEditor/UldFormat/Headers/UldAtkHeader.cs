using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldAtkHeader : UldGenericHeader {
        public uint AssetOffset;
        public uint PartOffset;
        public uint ComponentOffset;
        public uint TimelineOffset;
        public uint WidgetOffset; // ?
        public uint RewriteDataOffset; // ?
        public uint TimelineSize;

        public UldAtkHeader( BinaryReader reader ) : base( reader ) {
            AssetOffset = reader.ReadUInt32();
            PartOffset = reader.ReadUInt32();
            ComponentOffset = reader.ReadUInt32();
            TimelineOffset = reader.ReadUInt32();
            WidgetOffset = reader.ReadUInt32();
            RewriteDataOffset = reader.ReadUInt32();
            TimelineSize = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer, out long offsetsPosition ) {
            WriteHeader( writer );
            offsetsPosition = writer.BaseStream.Position;
            writer.Write( AssetOffset );
            writer.Write( PartOffset );
            writer.Write( ComponentOffset );
            writer.Write( TimelineOffset );
            writer.Write( WidgetOffset );
            writer.Write( RewriteDataOffset );
            writer.Write( TimelineSize );
        }
    }
}
