using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override void Draw( string parentId ) {
            Version.Draw( parentId, CommandManager.Scd );

            for( var idx = 0; idx < Channels.Count; idx++ ) {
                Channels[idx].Draw( $"{parentId}{idx}" );
            }
        }
    }
}
