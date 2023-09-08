using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldMainHeader : UldGenericHeader {
        public readonly uint ComponentOffset;
        public readonly uint WidgetOffset;

        public UldMainHeader( BinaryReader reader ) : base( reader ) {
            ComponentOffset = reader.ReadUInt32();
            WidgetOffset = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer, out long offsetsPosition ) {
            WriteHeader( writer );
            offsetsPosition = writer.BaseStream.Position;
            // Placeholders
            writer.Write( 0 );
            writer.Write( 0 );
        }

        public static void UpdateOffsets( BinaryWriter writer, long offsetsPosition, uint widgetOffset ) {
            writer.BaseStream.Position = offsetsPosition;
            writer.Write( 0x10 ); // Component offset
            writer.Write( widgetOffset );
        }
    }
}
