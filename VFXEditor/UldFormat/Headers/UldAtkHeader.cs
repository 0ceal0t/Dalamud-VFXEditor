using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldAtkHeader : UldGenericHeader {
        public readonly uint AssetOffset;
        public readonly uint PartOffset;
        public readonly uint ComponentOffset;
        public readonly uint TimelineOffset;
        public readonly uint WidgetOffset; // ?
        public readonly uint RewriteDataOffset; // ?
        public readonly uint TimelineSize; // TODO: this is weird sometimes

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
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( RewriteDataOffset );
            writer.Write( TimelineSize );
        }

        public static void UpdateOffsets( BinaryWriter writer, long offsetsPosition, uint assetOffset, uint partOffset, uint componentOffset, uint timelineOffset ) {
            writer.BaseStream.Position = offsetsPosition;
            writer.Write( assetOffset );
            writer.Write( partOffset );
            writer.Write( componentOffset );
            writer.Write( timelineOffset );
        }
    }
}
