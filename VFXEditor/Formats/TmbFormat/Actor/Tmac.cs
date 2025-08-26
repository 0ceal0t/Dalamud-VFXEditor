using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Formats.TmbFormat.Track;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Actor {
    public class Tmac : TmbItemWithTime {
        public override string Magic => "TMAC";
        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt AbilityDelay = new( "Ability Delay" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public readonly List<Tmtr> Tracks = [];
        private readonly TmbTrackSplitView TrackView;

        public DangerLevel MaxDanger => Tracks.Count == 0 ? DangerLevel.None : Tracks.Select( x => x.MaxDanger ).Max();
        public int AllTracksIdx => Tracks.Count == 0 ? 0 : Tracks.Max( File.AllTracks.IndexOf ) + 1;
        private readonly List<int> TempIds;

        public Tmac( TmbFile file ) : base( file ) {
            TrackView = new( this );
        }

        public Tmac( TmbFile file, TmbReader reader ) : base( file, reader ) {
            AbilityDelay.Read( reader );
            Unk2.Read( reader );
            TempIds = reader.ReadOffsetTimeline();

            TrackView = new( this );
        }

        public void PickTracks( TmbReader reader ) {
            Tracks.AddRange( reader.Pick<Tmtr>( TempIds ) );
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            AbilityDelay.Write( writer );
            Unk2.Write( writer );
            writer.WriteOffsetTimeline( Tracks );
        }

        public void Draw() {
            DrawHeader();
            AbilityDelay.Draw();
            Unk2.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            TrackView.Draw();
        }

        // ====== TRACK MANIPLUATION ==========

        public void ImportTrack( byte[] data ) {
            TmbReader.Import( File, data, out var _, out var tracks, out var entries, out var _, true );
            Dalamud.Log( $"{tracks.Count} {entries.Count}" );
            var commands = new List<ICommand>();
            var track = tracks[0];
            AddTrack( commands, track );
            track.Entries.AddRange( entries );
            commands.Add( new ListAddRangeCommand<TmbEntry>( File.AllEntries, entries ) );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        public void AddTrack() {
            var commands = new List<ICommand>();
            AddTrack( commands, new Tmtr( File ) );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        public void AddTrack( List<ICommand> commands, Tmtr track ) {
            commands.Add( new ListAddCommand<Tmtr>( Tracks, track ) );
            commands.Add( new ListAddCommand<Tmtr>( File.AllTracks, track, AllTracksIdx ) );
        }

        public void DeleteTrack( Tmtr track ) {
            if( !Tracks.Contains( track ) ) return;
            var commands = new List<ICommand>();
            DeleteTrack( commands, track );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        public void DeleteTrack( List<ICommand> commands, Tmtr track ) {
            if( !Tracks.Contains( track ) ) return;
            commands.Add( new ListRemoveCommand<Tmtr>( Tracks, track ) );
            commands.Add( new ListRemoveCommand<Tmtr>( File.AllTracks, track ) );
            track.DeleteAllEntries( commands );
        }

        public void DeleteChildren( List<ICommand> commands, TmbFile file ) {
            foreach( var track in Tracks ) {
                commands.Add( new ListRemoveCommand<Tmtr>( Tracks, track ) );
                commands.Add( new ListRemoveCommand<Tmtr>( file.AllTracks, track ) );
                track.DeleteAllEntries( commands );
            }
        }
    }
}
