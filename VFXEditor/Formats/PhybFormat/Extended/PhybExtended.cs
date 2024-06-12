using System.IO;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public const uint MAGIC_PACK = 0x4B434150;
        public const uint MAGIC_EPHB = 0x42485045;

        private readonly byte[] EphbData;
        private readonly byte[] PackData;

        public int Size => EphbData.Length + 0x10 + PackData.Length;

        public PhybExtended() { }

        public PhybExtended( BinaryReader reader ) : this() {
            var ephbCount = 0;
            for( var pos = reader.BaseStream.Length - 0x18; pos >= 0; pos-- ) { // go backwards from the end
                reader.BaseStream.Position = pos;
                if( reader.ReadUInt32() == MAGIC_EPHB ) {
                    ephbCount++;
                    if( ephbCount == 2 ) { // found it. kinda jank but it works
                        reader.ReadUInt32(); // 1
                        var size = reader.ReadUInt32();
                        reader.ReadUInt32(); // 0
                        EphbData = reader.ReadBytes( ( int )size );
                        PackData = reader.ReadBytes( 0x18 );
                        break;
                    }
                }
            }

            if( EphbData == null ) {
                Dalamud.Error( "Could not find EPHB data" );
                return;
            }

            // TODO: read flatbuffers
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( MAGIC_EPHB );
            writer.Write( 1 );
            writer.Write( EphbData.Length );
            writer.Write( 0 );
            writer.Write( EphbData );
            writer.Write( PackData );
        }
    }
}
