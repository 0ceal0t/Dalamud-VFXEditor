using Dalamud.Interface;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.FileManager;

namespace VfxEditor.TmbFormat {
    public class Tmac : TmbItemWithTime {
        public override string Magic => "TMAC";
        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public readonly List<Tmtr> Tracks = new();
        private Tmtr SelectedTrack = null;

        private readonly List<int> TempIds;

        public Tmac( bool papEmbedded ) : base( papEmbedded ) { }

        public Tmac( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            TempIds = reader.ReadOffsetTimeline();
        }

        public void PickTracks( TmbReader reader ) {
            Tracks.AddRange( reader.Pick<Tmtr>( TempIds ) );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            writer.WriteOffsetTimeline( Tracks );
        }

        public void Draw( string id, List<Tmtr> tracksMaster, List<TmbEntry> entriesMaster ) {
            DrawTime( id );
            Unk1.Draw( id, Command );
            Unk2.Draw( id, Command );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            ImGui.BeginChild( $"{id}-ActorChild", new Vector2( -1, -1 ), true );
            ImGui.Columns( 2, $"{id}-ActorChild-Cols", true );

            // Left column

            ImGui.BeginChild( $"{id}-ActorChild-Left" );
            ImGui.PushFont( UiBuilder.IconFont );

            // New
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                var newTrack = new Tmtr( PapEmbedded );
                var idx = Tracks.Count == 0 ? 0 : tracksMaster.IndexOf( Tracks.Last() ) + 1;
                CompoundCommand command = new( false, true );
                command.Add( new GenericAddCommand<Tmtr>( tracksMaster, newTrack, idx ) );
                command.Add( new GenericAddCommand<Tmtr>( Tracks, newTrack ) );
                Command.Add( command );
            }

            // Remove
            if( SelectedTrack != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    CompoundCommand command = new( false, true );
                    command.Add( new GenericRemoveCommand<Tmtr>( Tracks, SelectedTrack ) );
                    command.Add( new GenericRemoveCommand<Tmtr>( tracksMaster, SelectedTrack ) );
                    Command.Add( command );

                    SelectedTrack = null;
                }
            }
            ImGui.PopFont();

            var selectedIndex = SelectedTrack == null ? -1 : Tracks.IndexOf( SelectedTrack );
            for( var i = 0; i < Tracks.Count; i++ ) {
                if( ImGui.Selectable( $"Track {i}{id}{i}", Tracks[i] == SelectedTrack ) ) {
                    SelectedTrack = Tracks[i];
                    selectedIndex = i;
                }
            }
            if( selectedIndex == -1 ) SelectedTrack = null;

            ImGui.EndChild();
            ImGui.SetColumnWidth( 0, 150 );

            // Right column

            ImGui.NextColumn();
            ImGui.BeginChild( $"{id}-ActorChild-Right" );

            if( SelectedTrack != null ) SelectedTrack.Draw( $"{id}{selectedIndex}", entriesMaster );
            else ImGui.Text( "Select a timeline track..." );

            ImGui.EndChild();

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }
    }
}
