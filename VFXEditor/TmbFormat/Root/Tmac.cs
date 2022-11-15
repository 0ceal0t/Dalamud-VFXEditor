using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    public class Tmac : TmbItemWithTime {
        public override string Magic => "TMAC";
        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public readonly List<Tmtr> Tracks = new();
        private readonly List<int> TempIds;

        private Tmtr SelectedTrack = null;

        public Tmac() { }

        public Tmac( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader.Reader, 4 );
            Unk2.Read( reader.Reader, 4 );
            TempIds = reader.ReadOffsetTimeline();
        }

        public void PickTracks( TmbReader reader ) {
            Tracks.AddRange( reader.Pick<Tmtr>( TempIds ) );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer.Writer );
            Unk2.Write( writer.Writer );
            writer.WriteOffsetTimeline( Tracks );
        }

        public void Draw( string id, List<Tmtr> tracksMaster, List<TmbEntry> entriesMaster ) {
            DrawTime( id );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            ImGui.BeginChild( $"{id}-ActorChild", new Vector2( -1, -1 ), true );
            ImGui.Columns( 2, $"{id}-ActorChild-Cols", true );

            // Left column

            ImGui.BeginChild( $"{id}-ActorChild-Left" );
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                var newTrack = new Tmtr();
                if( Tracks.Count == 0 ) {
                    tracksMaster.Add( newTrack );
                }
                else {
                    var idx = tracksMaster.IndexOf( Tracks.Last() );
                    tracksMaster.Insert( idx + 1, newTrack );
                }
                Tracks.Add( newTrack );
            }
            if( SelectedTrack != null ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 20 );
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    Tracks.Remove( SelectedTrack );
                    tracksMaster.Remove( SelectedTrack );
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
