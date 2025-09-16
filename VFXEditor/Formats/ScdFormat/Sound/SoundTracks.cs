using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.Tables;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public class SoundTracks {
        public readonly List<SoundTrackInfo> Entries = [];
        private readonly CommandTable<SoundTrackInfo> EntryTable;

        public SoundTracks() {
            EntryTable = new( "Entries", true, false, Entries, [
                ( "Track Index", ImGuiTableColumnFlags.None, -1 ),
                ( "Audio Index", ImGuiTableColumnFlags.None, -1 ),
            ],
            () => new() );
        }

        public void Read( BinaryReader reader, byte entryCount ) {
            for( var i = 0; i < entryCount; i++ ) {
                var newEntry = new SoundTrackInfo();
                newEntry.Read( reader );
                Entries.Add( newEntry );
            }
        }

        public void Write( BinaryWriter writer ) {
            Entries.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            EntryTable.Draw();
        }
    }

    public class SoundTrackInfo : IUiItem {
        public readonly ParsedShort TrackIdx = new( "##Track" );
        public readonly ParsedShort AudioIdx = new( "##Audio" );

        public void Read( BinaryReader reader ) {
            TrackIdx.Read( reader );
            AudioIdx.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TrackIdx.Write( writer );
            AudioIdx.Write( writer );
        }

        public void Draw() {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            TrackIdx.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            AudioIdx.Draw();
        }
    }
}
