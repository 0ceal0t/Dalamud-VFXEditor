using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data.Command.ListCommands;
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

        public readonly List<Tmtr> Tracks = new();
        private Tmtr SelectedTrack = null;

        public DangerLevel MaxDanger => Tracks.Count == 0 ? DangerLevel.None : Tracks.Select( x => x.MaxDanger ).Max();

        private readonly List<int> TempIds;

        public Tmac( TmbFile file ) : base( file ) { }

        public Tmac( TmbFile file, TmbReader reader ) : base( file, reader ) {
            AbilityDelay.Read( reader );
            Unk2.Read( reader );
            TempIds = reader.ReadOffsetTimeline();
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

            ImGui.Columns( 2, "Cols", true );

            var selectedIndex = SelectedTrack == null ? -1 : Tracks.IndexOf( SelectedTrack );
            if( selectedIndex == -1 ) SelectedTrack = null;

            // Left column

            using( var left = ImRaii.Child( "Left" ) ) {
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) { // NEW
                        var newTrack = new Tmtr( File );
                        var idx = Tracks.Count == 0 ? 0 : File.Tracks.IndexOf( Tracks.Last() ) + 1;

                        var commands = new List<ICommand> {
                            new ListAddCommand<Tmtr>( Tracks, newTrack ),
                            new ListAddCommand<Tmtr>( File.Tracks, newTrack, idx )
                        };
                        CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
                    }

                    if( SelectedTrack != null ) {
                        ImGui.SameLine();
                        if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) { // REMOVE
                            var commands = new List<ICommand> {
                                new ListRemoveCommand<Tmtr>( Tracks, SelectedTrack ),
                                new ListRemoveCommand<Tmtr>( File.Tracks, SelectedTrack )
                            };
                            SelectedTrack.DeleteAllEntries( commands );
                            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );

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

                    if( ImGui.BeginDragDropTarget() ) {
                        File.StopDragging( Tracks[idx] );
                        ImGui.EndDragDropTarget();
                    }
                }
            }

            ImGui.SetColumnWidth( 0, 150 );

            // Right column

            ImGui.NextColumn();

            using( var right = ImRaii.Child( "Right" ) ) {
                if( SelectedTrack != null ) {
                    using var _ = ImRaii.PushId( selectedIndex );
                    SelectedTrack.Draw();
                }
                else ImGui.Text( "Select a timeline track..." );
            }

            ImGui.Columns( 1 );
        }

        public void DeleteChildren( List<ICommand> commands, TmbFile file ) { // file.RefreshIds();
            foreach( var track in Tracks ) {
                commands.Add( new ListRemoveCommand<Tmtr>( Tracks, track ) );
                commands.Add( new ListRemoveCommand<Tmtr>( file.Tracks, track ) );
                track.DeleteAllEntries( commands );
            }
        }
    }
}
