using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components.Tables;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public class SoundRandomTracks {
        public readonly List<RandomTrackInfo> Entries = [];
        private readonly CommandTable<RandomTrackInfo> EntryTable;

        public readonly ParsedInt CycleInterval = new( "Cycle Interval" );
        public readonly ParsedShort CycleNumPlayTrack = new( "Cycle Play Track" );
        public readonly ParsedShort CycleRange = new( "Cycle Range" );

        public SoundRandomTracks() {
            EntryTable = new( "Entries", true, false, Entries, [
                ( "Track Index", ImGuiTableColumnFlags.None, -1 ),
                ( "Audio Index", ImGuiTableColumnFlags.None, -1 ),
                ( "Limit", ImGuiTableColumnFlags.None, -1 ),
            ],
            () => new() );
        }

        public void Read( BinaryReader reader, SoundType type, byte trackCount ) {
            for( var i = 0; i < trackCount; i++ ) {
                var newTrack = new RandomTrackInfo();
                newTrack.Read( reader );
                Entries.Add( newTrack );
            }

            if( type == SoundType.Cycle ) {
                CycleInterval.Read( reader );
                CycleNumPlayTrack.Read( reader );
                CycleRange.Read( reader );
            }
        }

        public void Write( BinaryWriter writer, SoundType type ) {
            Entries.ForEach( x => x.Write( writer ) );

            if( type == SoundType.Cycle ) {
                CycleInterval.Write( writer );
                CycleNumPlayTrack.Write( writer );
                CycleRange.Write( writer );
            }
        }

        public void Draw( SoundType type ) {
            using var _ = ImRaii.PushId( "Tracks" );

            if( type == SoundType.Cycle ) {
                CycleInterval.Draw();
                CycleNumPlayTrack.Draw();
                CycleRange.Draw();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            EntryTable.Draw();
        }
    }

    public class RandomTrackInfo : IUiItem {
        public readonly SoundTrackInfo Track = new();
        public readonly ParsedShort2 Limit = new( "##Limit" );

        public void Read( BinaryReader reader ) {
            Track.Read( reader );
            Limit.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Track.Write( writer );
            Limit.Write( writer );
        }

        public void Draw() {
            Track.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 150 );
            Limit.Draw();
        }
    }
}
