using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackVolumeZeroOneData : ScdTrackData {
        public readonly ParsedByte Version = new( "Version" );
        private byte Reserved1;
        private short HeaderSize;
        private readonly List<TrackZeroOnePoint> Points = new();

        public override void Read( BinaryReader reader ) {
            Version.Read( reader );
            Reserved1 = reader.ReadByte();
            HeaderSize = reader.ReadInt16();

            var count = reader.ReadInt16();
            for( var i = 0; i < count; i++ ) {
                var newPoint = new TrackZeroOnePoint();
                newPoint.Read( reader );
                Points.Add( newPoint );
            }
        }

        public override void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Reserved1 );
            writer.Write( HeaderSize );

            writer.Write( ( short )Points.Count );
            Points.ForEach( x => x.Write( writer ) );
        }

        public override void Draw() {
            Version.Draw( CommandManager.Scd );

            for( var idx = 0; idx < Points.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                Points[idx].Draw();
            }
        }
    }

    public class TrackZeroOnePoint {
        public readonly ParsedShort ZeroOne = new( "Zero One" );
        public readonly ParsedShort Value = new( "Value " );

        public void Read( BinaryReader reader ) {
            ZeroOne.Read( reader );
            Value.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            ZeroOne.Write( writer );
            Value.Write( writer );
        }

        public void Draw() {
            ZeroOne.Draw( CommandManager.Scd );
            Value.Draw( CommandManager.Scd );
        }
    }
}
