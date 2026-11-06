using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Formats.ScdFormat.Sound;

namespace VfxEditor.Formats.ScdFormat.Music {
    public class ScdAudioRemoveCommand : ListRemoveCommand<ScdAudioEntry> {
        private readonly List<SoundTrackInfo> Shifted = [];

        public ScdAudioRemoveCommand( ScdFile file, ScdAudioEntry item ) : base( file.Audio, item ) {
            item.Player.Reset();
            if( Idx == -1 ) return;

            foreach( var sound in file.Sounds ) {
                foreach( var track in sound.Tracks.Entries ) {
                    if( track.AudioIdx.Value > Idx ) Shifted.Add( track );
                }
                foreach( var random in sound.RandomTracks.Entries ) {
                    if( random.Track.AudioIdx.Value > Idx ) Shifted.Add( random.Track );
                }
            }
            Shift( -1 );
        }

        public override void Redo() {
            base.Redo();
            Item.Player.Reset();
            Shift( -1 );
        }

        public override void Undo() {
            base.Undo();
            Shift( 1 );
        }

        private void Shift( int amount ) {
            foreach( var track in Shifted ) track.AudioIdx.Value += amount;
        }
    }
}