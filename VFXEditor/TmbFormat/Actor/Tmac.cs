using Dalamud.Interface;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.FileManager;

namespace VfxEditor.TmbFormat.Actor {
    public class Tmac : TmbItemWithTime {
        public override string Magic => "TMAC";
        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Ability Delay" );
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

        public void Draw( string id, TmbFile file ) {
            DrawHeader( id );
            Unk1.Draw( id, Command );
            Unk2.Draw( id, Command );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            ImGui.BeginChild( $"{id}/Child", new Vector2( -1, -1 ), true );
            ImGui.Columns( 2, $"{id}/ChildCols", true );

            // Left column

            ImGui.BeginChild( $"{id}/Left" );
            ImGui.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) { // NEW
                var newTrack = new Tmtr( PapEmbedded );
                var idx = Tracks.Count == 0 ? 0 : file.Tracks.IndexOf( Tracks.Last() ) + 1;

                TmbRefreshIdsCommand command = new( file, false, true );
                command.Add( new GenericAddCommand<Tmtr>( Tracks, newTrack ) );
                command.Add( new GenericAddCommand<Tmtr>( file.Tracks, newTrack, idx ) );
                Command.Add( command );
            }

            if( SelectedTrack != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) { // REMOVE
                    TmbRefreshIdsCommand command = new( file, false, true );
                    command.Add( new GenericRemoveCommand<Tmtr>( Tracks, SelectedTrack ) );
                    command.Add( new GenericRemoveCommand<Tmtr>( file.Tracks, SelectedTrack ) );
                    SelectedTrack.DeleteChildren( command, file );
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
            ImGui.BeginChild( $"{id}/Right" );

            if( SelectedTrack != null ) SelectedTrack.Draw( $"{id}{selectedIndex}", file );
            else ImGui.Text( "Select a timeline track..." );

            ImGui.EndChild();

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }

        public void DeleteChildren( TmbRefreshIdsCommand command, TmbFile file ) {
            foreach( var track in Tracks ) {
                command.Add( new GenericRemoveCommand<Tmtr>( Tracks, track ) );
                command.Add( new GenericRemoveCommand<Tmtr>( file.Tracks, track ) );
                track.DeleteChildren( command, file );
            }
        }
    }
}
