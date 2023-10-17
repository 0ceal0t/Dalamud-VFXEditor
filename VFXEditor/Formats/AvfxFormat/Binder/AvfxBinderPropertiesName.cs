using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AvfxFormat.Binder {
    public class AvfxBinderPropertiesName : AvfxString {
        public AvfxBinderPropertiesName() : base( "Name", "Name", true, true ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size == 0 ) return;
            Parsed.Read( reader );
            reader.ReadBytes( 3 );
            FileUtils.PadTo( reader, 4 );
        }

        public override void WriteContents( BinaryWriter writer ) {
            if( string.IsNullOrEmpty( Parsed.Value ) ) return;
            Parsed.Write( writer );
            writer.Write( new byte[3] );
            FileUtils.PadTo( writer, 4 );
        }
    }
}
