using System.IO;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public const uint MAGIC_PACK = 0x4B434150;
        public const uint MAGIC_EPHB = 0x42485045;

        private readonly byte[] EphbData;
        private readonly byte[] PackData;

        public int Size => EphbData.Length + 0x10 + PackData.Length;

        public readonly PhybEphbTable Table;

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

            Table = new( EphbTable.GetRootAsEphbTable( new( EphbData ) ).UnPack() );

            // TODO: pack data
        }

        public void Write( BinaryWriter writer ) {
            var data = Table.Export().SerializeToBinary();

            writer.Write( MAGIC_EPHB );
            writer.Write( 1 );
            writer.Write( data.Length );
            writer.Write( 0 );
            writer.Write( data );
            writer.Write( PackData );
        }

        public void Draw() => Table.Draw();
    }
}
