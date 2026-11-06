using VfxEditor.Data.Command;

namespace VfxEditor.Formats.ScdFormat.Music {
    public class ScdAudioReplaceCommand : ICommand {
        private readonly ScdFile File;
        private readonly ScdAudioEntry OldEntry;
        private readonly ScdAudioEntry NewEntry;

        public ScdAudioReplaceCommand( ScdFile file, ScdAudioEntry oldEntry, ScdAudioEntry newEntry ) {
            File = file;
            OldEntry = oldEntry;
            NewEntry = newEntry;
            Swap( OldEntry, NewEntry );
        }

        public void Redo() => Swap( OldEntry, NewEntry );

        public void Undo() => Swap( NewEntry, OldEntry );

        private void Swap( ScdAudioEntry from, ScdAudioEntry to ) {
            var index = File.Audio.IndexOf( from );
            if( index == -1 ) return;
            File.Audio[index] = to;
            from.Player.Reset(); // stop playback, but keep the entry usable in case this is undone
            to.Marker.SetEntry( to ); // the marker is shared between the old and new entries
            File.AudioSplitView.SwapSelected( from, to );
        }
    }
}