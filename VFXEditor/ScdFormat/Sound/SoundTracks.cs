using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class SoundTrackInfo {
        public readonly ParsedShort TrackIdx = new( "Track Index" );
        public readonly ParsedShort AudioIdx = new( "Audio Index" );

        public void Read( BinaryReader reader ) {
            TrackIdx.Read( reader );
            AudioIdx.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TrackIdx.Write( writer );
            AudioIdx.Write( writer );
        }
    }

    public class SoundTracks {
        public List<SoundTrackInfo> Tracks = new();

        public void Read( BinaryReader reader, byte trackCount ) {
            for( var i = 0; i < trackCount; i++ ) {
                var newTrack = new SoundTrackInfo();
                newTrack.Read( reader );
                Tracks.Add( newTrack );
            }
        }

        public void Write( BinaryWriter writer ) {
            Tracks.ForEach( x => x.Write( writer ) );
        }
    }

    public class SoundRandomTracks {
        public List<RandomTrackInfo> Tracks = new();
        public readonly ParsedInt CycleInterval = new( "Cycle Interval" );
        public readonly ParsedShort CycleNumPlayTrack = new( "Cycle Play Track" );
        public readonly ParsedShort CycleRange = new( "Cycle Range" );

        public void Read( BinaryReader reader, SoundType type, byte trackCount ) {
            for( var i = 0; i < trackCount; i++ ) {
                var newTrack = new RandomTrackInfo();
                newTrack.Read( reader );
                Tracks.Add( newTrack );
            }

            if( type == SoundType.Cycle ) {
                CycleInterval.Read( reader );
                CycleNumPlayTrack.Read( reader );
                CycleRange.Read( reader );
            }
        }

        public void Write( BinaryWriter writer, SoundType type ) {
            Tracks.ForEach( x => x.Write( writer ) );

            if( type == SoundType.Cycle ) {
                CycleInterval.Write( writer );
                CycleNumPlayTrack.Write( writer );
                CycleRange.Write( writer );
            }
        }
    }

    public class RandomTrackInfo {
        public readonly SoundTrackInfo Track = new();
        public readonly ParsedInt UpperLimit = new( "Upper Limit" );

        public void Read( BinaryReader reader ) {
            Track.Read( reader );
            UpperLimit.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Track.Write( writer );
            UpperLimit.Write( writer );
        }
    }
}
