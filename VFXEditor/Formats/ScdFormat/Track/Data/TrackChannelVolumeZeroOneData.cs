using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackChannelVolumeZeroOneData : ScdTrackData {
        public readonly ParsedByte Version = new( "Version" );
        private byte Reserved1;
        private short HeaderSize;
        private readonly List<TrackVolumeZeroOneData> Channels = new();

        public override void Read( BinaryReader reader ) {
            Version.Read( reader );
            Reserved1 = reader.ReadByte();
            HeaderSize = reader.ReadInt16();

            var count = reader.ReadInt16();
            for( var i = 0; i < count; i++ ) {
                var newPoint = new TrackVolumeZeroOneData();
                newPoint.Read( reader );
                Channels.Add( newPoint );
            }
        }

        public override void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( Reserved1 );
            writer.Write( HeaderSize );

            writer.Write( ( short )Channels.Count );
            Channels.ForEach( x => x.Write( writer ) );
        }

        public override void Draw() {
            Version.Draw();

            for( var idx = 0; idx < Channels.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                Channels[idx].Draw();
            }
        }
    }
}
