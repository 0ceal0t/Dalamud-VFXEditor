using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ScdFormat.Music.Marker;
using VfxEditor.Parsing;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public enum SscfWaveFormat : int {
        Empty = -1,
        Pcm = 0x01,
        Atrac3 = 0x05,
        Vorbis = 0x06,
        Xma = 0x0B,
        MsAdPcm = 0x0C,
        Atrac3Too = 0x0D
    }

    [Flags]
    public enum AudioFlag {
        Enabled_Marker = 0x01,
        Mono_Split = 0x02,
        Version_Shift = 0x01000000
    }

    public class ScdAudioEntry : ScdEntry, IUiItem {
        public readonly ScdFile File;
        public readonly AudioPlayer Player;
        public ScdAudioData Data;

        public int DataLength;
        public int NumChannels;
        public int SampleRate;
        public SscfWaveFormat Format;
        public int LoopStart = 0;
        public int LoopEnd = 0;

        private Vector2 _LoopTimeInternal = new( -1, -1 );
        public Vector2 LoopTime {
            get {
                try {
                    if( _LoopTimeInternal.X < 0 ) _LoopTimeInternal = Data.GetLoopTime();
                }
                catch( Exception ) {
                    _LoopTimeInternal = new( 0, 0 );
                }
                return _LoopTimeInternal;
            }
            set {
                LoopStart = Data.TimeToBytes( value.X );
                LoopEnd = Data.TimeToBytes( value.Y );
                _LoopTimeInternal = Data.GetLoopTime();
            }
        }

        public readonly ParsedFlag<AudioFlag> Flags = new( "Flags" );
        public ScdAudioMarker Marker = new();

        public bool HasMarker => Flags.HasFlag( AudioFlag.Enabled_Marker );
        public int AuxDataSize => HasMarker ? Marker.GetSize() : 0;

        public ScdAudioEntry( ScdFile file ) {
            Player = new( this );
            File = file;
            Marker.SetEntry( this );
        }

        public ScdAudioEntry( ScdAudioEntry baseEntry, int dataLength, int numChannel, int sampleRate, SscfWaveFormat format ) : this( baseEntry.File ) {
            DataLength = dataLength;
            NumChannels = numChannel;
            SampleRate = sampleRate;
            Format = format;
            Flags.Value = baseEntry.Flags.Value;
            Marker = baseEntry.Marker;
            Marker.SetEntry( this );
        }

        public override void Read( BinaryReader reader ) {
            DataLength = reader.ReadInt32();
            NumChannels = reader.ReadInt32();
            SampleRate = reader.ReadInt32();
            Format = ( SscfWaveFormat )reader.ReadInt32();
            LoopStart = reader.ReadInt32();
            LoopEnd = reader.ReadInt32();
            var subInfoSize = reader.ReadInt32();
            Flags.Read( reader );

            if( DataLength == 0 ) return;

            // SubInfoSize is from here
            if( HasMarker ) Marker.Read( reader );

            Data = Format switch {
                SscfWaveFormat.MsAdPcm => new ScdAdpcm( reader, subInfoSize - AuxDataSize, this ),
                SscfWaveFormat.Vorbis => new ScdVorbis( reader, this ),
                _ => null
            };
        }

        public void Dispose() {
            Player.Dispose();
            Data = null;
        }

        public override void Write( BinaryWriter writer ) => Write( writer, out var _ );

        public void Write( BinaryWriter writer, out long padding ) {
            writer.Write( DataLength );
            writer.Write( NumChannels );
            writer.Write( SampleRate );
            writer.Write( ( int )Format );
            writer.Write( LoopStart );
            writer.Write( LoopEnd );
            writer.Write( AuxDataSize + ( Data == null ? 0 : Data.GetSubInfoSize() ) );
            Flags.Write( writer );
            if( HasMarker ) Marker.Write( writer );
            Data?.Write( writer );

            padding = FileUtils.PadTo( writer, 16 );
        }

        public void Draw() {
            if( DataLength == 0 ) return;

            var audioIndex = File.Audio.IndexOf( this );
            var soundIndexes = new List<int>();
            foreach( var (sound, index) in File.Sounds.WithIndex() ) {
                if( sound.Tracks.Entries.FindFirst( x => x.AudioIdx.Value == audioIndex, out var _ ) ) soundIndexes.Add( index );
            }

            if( soundIndexes.Count > 0 ) {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                using var disabled = ImRaii.Disabled();
                ImGui.Text( "Sounds: " );
                foreach( var idx in soundIndexes ) {
                    ImGui.SameLine();
                    ImGui.SmallButton( $"{idx}" );
                }
            }

            Player.Draw();
        }

        public void DrawTabs() {
            if( ImGui.BeginTabItem( "Parameters" ) ) {
                using( var child = ImRaii.Child( "Child" ) ) {
                    Flags.Draw();
                }
                ImGui.EndTabItem();
            }
            if( HasMarker && ImGui.BeginTabItem( "Markers" ) ) {
                Marker.Draw();
                ImGui.EndTabItem();
            }
        }

        // ==========================================

        public static ScdAudioEntry Default( ScdFile file ) {
            var audio = new ScdAudioEntry( file );
            using var reader = new BinaryReader( System.IO.File.Open( Path.Combine( Plugin.RootLocation, "Files", "default_scd_audio.scd" ), FileMode.Open ) );
            audio.Read( reader );
            return audio;
        }
    }
}
