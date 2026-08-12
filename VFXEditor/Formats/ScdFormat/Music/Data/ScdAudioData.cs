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

        // Bytes are the SCD entry's own loop-point unit; samples are the unit used by
        // OGG loop tags. Both are exact/precise; time (seconds) is a derived display unit.
        public abstract int SamplesToBytes( int samples );

        public abstract int BytesToSamples( int bytes );

        public abstract int TimeToBytes( float time );

        public abstract float BytesToTime( int bytes );

        public abstract Vector2 GetLoopTime();

        public abstract void Write( BinaryWriter writer );

        public abstract int GetSubInfoSize();

        public delegate ScdAudioEntry GetAudioEntryDelegate( string path, ScdAudioEntry oldEntry );

        public abstract Dictionary<string, GetAudioEntryDelegate> GetImportActions();

        public abstract string GetDefaultExtension();

        public abstract byte[] GetDefaultExtensionData();
    }
}
