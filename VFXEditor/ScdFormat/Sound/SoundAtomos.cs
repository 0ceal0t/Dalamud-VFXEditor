using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class SoundAtomos {
        public readonly ParsedByte Version = new( "Version" );
        private byte Reserved1;
        private ushort Size;
        public readonly ParsedShort MinPeople = new( "Minimum Number of People" );
        public readonly ParsedShort MaxPeople = new( "Maximum Number of People" );
        private readonly ParsedReserve Reserve2 = new( 2 * 4 );

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Reserved1 = reader.ReadByte();
            Size = reader.ReadUInt16();
            MinPeople.Read( reader );
            MaxPeople.Read( reader );
            Reserve2.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Reserved1 );
            writer.Write( Size );
            MinPeople.Write( writer );
            MaxPeople.Write( writer );
            Reserve2.Write( writer );
        }
    }
}
