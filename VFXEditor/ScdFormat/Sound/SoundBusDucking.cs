using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class SoundBusDucking {
        private byte Size;
        public readonly ParsedByte Number = new( "Number" );
        private readonly ParsedReserve Reserve1 = new( 2 );
        public readonly ParsedInt FadeTime = new( "Fade Time" );
        public readonly ParsedFloat Volume = new( "Volume" );
        private uint Reserve2;

        public void Read( BinaryReader reader ) {
            Size = reader.ReadByte();
            Number.Read( reader );
            Reserve1.Read( reader );
            FadeTime.Read( reader );
            Volume.Read( reader );
            Reserve2 = reader.ReadUInt32();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Size );
            Number.Write( writer );
            Reserve1.Write( writer );
            FadeTime.Write( writer );
            Volume.Write( writer );
            writer.Write( Reserve2 );
        }
    }
}
