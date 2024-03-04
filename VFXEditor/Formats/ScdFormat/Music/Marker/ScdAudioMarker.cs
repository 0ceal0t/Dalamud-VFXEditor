using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat.Music.Marker {
    public class ScdAudioMarker {
        public readonly ParsedPaddedString Id = new( "Id", 4, 0x00 );
        public readonly ParsedInt LoopStart = new( "Loop Start" );
        public readonly ParsedInt LoopEnd = new( "Loop End" );

        public readonly List<ParsedInt> Markers = new();
        private readonly CommandListView<ParsedInt> MarkerView;

        public ScdAudioMarker() {
            MarkerView = new( Markers, () => new( "##Marker" ), true );
        }

        public int GetSize() {
            var size = ( 4 * 5 ) + ( Markers.Count * 4 );
            return size + ( int )FileUtils.NumberToPad( size, 16 );
        }

        public void Read( BinaryReader reader ) {
            Id.Value = FileUtils.ReadString( reader, 4 );
            reader.ReadUInt32();
            LoopStart.Read( reader );
            LoopEnd.Read( reader );
            var numMarkers = reader.ReadInt32();

            for( var i = 0; i < numMarkers; i++ ) {
                var newMarker = new ParsedInt( "##Marker" );
                newMarker.Read( reader );
                Markers.Add( newMarker );
            }

            FileUtils.PadTo( reader, 16 );
        }

        public void Write( BinaryWriter writer ) {
            FileUtils.WriteString( writer, Id.Value );
            writer.Write( GetSize() );
            LoopStart.Write( writer );
            LoopEnd.Write( writer );
            writer.Write( Markers.Count );
            foreach( var marker in Markers ) marker.Write( writer );

            FileUtils.PadTo( writer, 16 );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Marker" );
            Id.Draw();
            LoopStart.Draw();
            LoopEnd.Draw();

            ImGui.Separator();
            using var child = ImRaii.Child( "Child" );
            MarkerView.Draw();
        }
    }
}
