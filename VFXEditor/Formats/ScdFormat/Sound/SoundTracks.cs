using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class SoundTracks {
        public List<SoundTrackInfo> Entries = new();

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
            using var _ = ImRaii.PushId( "Entries" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            for( var idx = 0; idx < Entries.Count; idx++ ) {
                if( ImGui.CollapsingHeader( $"Entry #{idx}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var __ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();

                    if( UiUtils.RemoveButton( "Delete", true ) ) { // REMOVE
                        CommandManager.Add( new ListRemoveCommand<SoundTrackInfo>( Entries, Entries[idx] ) );
                    }

                    Entries[idx].Draw();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                }
            }

            if( ImGui.Button( "+ New" ) ) { // NEW
                CommandManager.Add( new ListAddCommand<SoundTrackInfo>( Entries, new SoundTrackInfo() ) );
            }
        }
    }

    public class SoundTrackInfo {
        public readonly ParsedShort TrackIdx = new( "Track Index" );
        public readonly ParsedShort AudioIdx = new( "Audio Index" );

        public void Read( BinaryReader reader ) {
            TrackIdx.Read( reader );
            AudioIdx.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TrackIdx.Write( writer );
            AudioIdx.Write( writer );
        }

        public void Draw() {
            TrackIdx.Draw();
            AudioIdx.Draw();
        }
    }
}
