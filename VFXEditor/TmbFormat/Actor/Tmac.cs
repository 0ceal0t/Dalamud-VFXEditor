using Dalamud.Interface;
using ImGuiNET;
using System.Linq;
using System.Collections.Generic;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.FileManager;
using VfxEditor.TmbFormat.Entries;
using OtterGui.Raii;

namespace VfxEditor.TmbFormat.Actor {
    public class Tmac : TmbItemWithTime {
        public override string Magic => "TMAC";
        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Ability Delay" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public readonly List<Tmtr> Tracks = new();
        private Tmtr SelectedTrack = null;

        public DangerLevel MaxDanger => Tracks.Count == 0 ? DangerLevel.None : Tracks.Select( x => x.MaxDanger ).Max();

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

        public void Draw( TmbFile file ) {
            DrawHeader();
            Unk1.Draw( Command );
            Unk2.Draw( Command );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var child = ImRaii.Child( "Child" );
            ImGui.Columns( 2, $"Cols", true );

            var selectedIndex = SelectedTrack == null ? -1 : Tracks.IndexOf( SelectedTrack );
            if( selectedIndex == -1 ) SelectedTrack = null;

            // Left column

            using( var left = ImRaii.Child( "Left" ) ) {
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" ) ) { // NEW
                        var newTrack = new Tmtr( PapEmbedded );
                        var idx = Tracks.Count == 0 ? 0 : file.Tracks.IndexOf( Tracks.Last() ) + 1;

                        TmbRefreshIdsCommand command = new( file, false, true );
                        command.Add( new GenericAddCommand<Tmtr>( Tracks, newTrack ) );
                        command.Add( new GenericAddCommand<Tmtr>( file.Tracks, newTrack, idx ) );
                        Command.Add( command );
                    }

                    if( SelectedTrack != null ) {
                        ImGui.SameLine();
                        if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" ) ) { // REMOVE
                            TmbRefreshIdsCommand command = new( file, false, true );
                            command.Add( new GenericRemoveCommand<Tmtr>( Tracks, SelectedTrack ) );
                            command.Add( new GenericRemoveCommand<Tmtr>( file.Tracks, SelectedTrack ) );
                            SelectedTrack.DeleteChildren( command, file );
                            Command.Add( command );

                            SelectedTrack = null;
                        }
                    }
                }

                for( var idx = 0; idx < Tracks.Count; idx++ ) {
                    using var _ = ImRaii.PushId( idx );

                    var isColored = TmbEntry.DoColor( Tracks[idx].MaxDanger, out var col );
                    using var color = ImRaii.PushColor( ImGuiCol.Text, col, isColored );

                    if( ImGui.Selectable( $"Track {idx}", Tracks[idx] == SelectedTrack ) ) {
                        SelectedTrack = Tracks[idx];
                        selectedIndex = idx;
                    }
                }
            }

            ImGui.SetColumnWidth( 0, 150 );

            // Right column

            ImGui.NextColumn();

            using( var right = ImRaii.Child( "Right" ) ) {
                if( SelectedTrack != null ) {
                    using var _ = ImRaii.PushId( selectedIndex );
                    SelectedTrack.Draw( file );
                }
                else ImGui.Text( "Select a timeline track..." );
            }

            ImGui.Columns( 1 );
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
