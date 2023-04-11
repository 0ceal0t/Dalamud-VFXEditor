using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldAtkHeader2 : UldAtkHeader {
        public UldAtkHeader2( BinaryReader reader ) : base( reader ) { }

        public static void UpdateOffsets( BinaryWriter writer, long offsetsPosition, uint widgetOffset ) {
            UpdateOffsets( writer, offsetsPosition, 0, 0, 0, 0 );
            writer.Write( widgetOffset );
        }
    }
}
