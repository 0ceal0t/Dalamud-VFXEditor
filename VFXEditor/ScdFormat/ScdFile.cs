using ImGuiNET;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat {
    public class ScdFile : FileManagerFile {
        private readonly ScdHeader Header;
        private readonly ScdOffsetsHeader OffsetsHeader;
        public List<ScdSoundEntry> Music = new();

        public ScdFile( BinaryReader reader, bool checkOriginal = true ) {
            Header = new( reader );
            OffsetsHeader = new( reader );

            // offsets lists 0/1/2/3/4
            // offset lists also padded to 16 bytes

            // In the file, table data is in order: 3, 0, 1, 4, 2
            // padded to 16 bytes between each table data (such as between 3/0)
            reader.BaseStream.Seek( OffsetsHeader.OffsetSound, SeekOrigin.Begin );
            for( var i = 0; i < OffsetsHeader.CountSound; i++ ) {
                Music.Add( new ScdSoundEntry( reader, reader.ReadInt32() ) );
            }
        }

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( $"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Sounds{id}" ) ) {
                    DrawSounds( id );
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        private void DrawSounds( string id ) {
            ImGui.BeginChild( $"{id}-Child" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            for( var idx = 0; idx < Music.Count; idx++ ) {
                Music[idx].Draw( id + idx );
            }
            ImGui.EndChild();
        }

        public override void Write( BinaryWriter writer ) {
            // TODO
        }

        public void Dispose() => Music.ForEach( x => x.Dispose() );
    }
}
