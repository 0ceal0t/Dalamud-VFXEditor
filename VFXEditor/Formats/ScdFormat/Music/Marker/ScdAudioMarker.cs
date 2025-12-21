using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;
using VfxEditor.ScdFormat;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat.Music.Marker {
    public class ScdAudioMarker {
        private ScdAudioEntry Entry;

        public readonly ParsedPaddedString Id = new( "Id", 4, 0x00 );
        public readonly ParsedFloat LoopStart = new( "Loop Start" );
        public readonly ParsedFloat LoopEnd = new( "Loop End" );

        public readonly List<ParsedDouble> Markers = [];
        private readonly CommandListView<ParsedDouble> MarkerView;

        public ScdAudioMarker() {
            MarkerView = new( Markers, () => new( "##Marker" ), true );
        }

        public void SetEntry( ScdAudioEntry entry ) {
            Entry = entry;
        }

        public int GetSize() {
            var size = ( 4 * 5 ) + ( Markers.Count * 4 );
            return size + ( int )FileUtils.NumberToPad( size, 16 );
        }

        public void Read( BinaryReader reader ) {
            Id.Value = FileUtils.ReadString( reader, 4 );
            reader.ReadUInt32();
            LoopStart.Value = ( float )reader.ReadInt32() / Entry.SampleRate;
            LoopEnd.Value = ( float )reader.ReadInt32() / Entry.SampleRate;
            var numMarkers = reader.ReadInt32();

            for( var i = 0; i < numMarkers; i++ ) {
                var newMarker = new ParsedDouble( "##Marker" ) {
                    Value = ( float )reader.ReadInt32() / Entry.SampleRate
                };
                Markers.Add( newMarker );
            }

            FileUtils.PadTo( reader, 16 );
        }

        public void Write( BinaryWriter writer ) {
            FileUtils.WriteString( writer, Id.Value );
            writer.Write( GetSize() );
            writer.Write( ( int )( LoopStart.Value * Entry.SampleRate ) );
            writer.Write( ( int )( LoopEnd.Value * Entry.SampleRate ) );
            writer.Write( Markers.Count );
            foreach( var marker in Markers ) writer.Write( ( int )Math.Round( marker.Value * Entry.SampleRate, MidpointRounding.AwayFromZero ) );

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
