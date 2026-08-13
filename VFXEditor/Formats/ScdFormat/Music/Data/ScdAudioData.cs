using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VfxEditor.ScdFormat.Music.Data {
    public abstract class ScdAudioData {
        public readonly ScdAudioEntry Entry;

        public ScdAudioData( ScdAudioEntry entry ) {
            Entry = entry;
        }

        public abstract WaveStream GetStream();

        // "Raw" is whatever unit the SCD entry's own LoopStart/LoopEnd fields are stored in -
        // this differs per codec (HCA: block index, Vorbis: byte offset, ADPCM: sample index).
        // Samples are the unit used by OGG loop tags. Both are exact/precise; time (seconds)
        // is a derived display unit.
        public abstract int SamplesToRaw( int samples );

        public abstract int RawToSamples( int raw );

        public abstract int TimeToRaw( float time );

        public abstract float RawToTime( int raw );

        public abstract Vector2 GetLoopTime();

        public abstract void Write( BinaryWriter writer );

        public abstract int GetSubInfoSize();

        public delegate ScdAudioEntry GetAudioEntryDelegate( string path, ScdAudioEntry oldEntry );

        public abstract Dictionary<string, GetAudioEntryDelegate> GetImportActions();

        public abstract string GetDefaultExtension();

        public abstract byte[] GetDefaultExtensionData();
    }
}
