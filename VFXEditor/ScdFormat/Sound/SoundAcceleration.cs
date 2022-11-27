using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class SoundAccelerationInfo {
        public readonly ParsedByte Version = new( "Version" );
        private byte Size;
        private readonly ParsedReserve Reserve1 = new( 2 * 8 );
        public readonly ParsedFloat Volume = new( "Volume" );
        public readonly ParsedInt UpTime = new( "Up Time" );
        public readonly ParsedInt DownTime = new( "Down Time" );

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Size = reader.ReadByte();
            Reserve1.Read( reader );
            Volume.Read( reader );
            UpTime.Read( reader );
            DownTime.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Size );
            Reserve1.Write( writer );
            Volume.Write( writer );
            UpTime.Write( writer );
            DownTime.Write( writer );
        }
    }

    public class SoundAcceleration {
        public readonly ParsedByte Version = new( "Version" );
        private byte Size;
        private byte NumAcceleration = 0;
        private readonly ParsedReserve Reserve1 = new( 1 + 4 * 3 );
        public List<SoundAccelerationInfo> Acceleration = new();

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Size = reader.ReadByte();
            NumAcceleration = reader.ReadByte();
            Reserve1.Read( reader );

            for( var i = 0; i < 4; i++ ) {
                var newAcceleration = new SoundAccelerationInfo();
                newAcceleration.Read( reader );
                Acceleration.Add( newAcceleration );
            }
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Size );
            writer.Write( NumAcceleration );
            Reserve1 .Write( writer );

            Acceleration.ForEach( x => x.Write( writer ) );
        }
    }
}
