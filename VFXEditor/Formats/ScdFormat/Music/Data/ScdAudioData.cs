using NAudio.Wave;
using System.IO;
using System.Numerics;

namespace VfxEditor.ScdFormat.Music.Data {
    public abstract class ScdAudioData {
        public readonly ScdAudioEntry Entry;

        public ScdAudioData( ScdAudioEntry entry ) {
            Entry = entry;
        }

        public abstract WaveStream GetStream();

        public abstract int SamplesToBytes( int samples );

        public abstract int TimeToBytes( float time );

        public abstract Vector2 GetLoopTime();

        public abstract void Write( BinaryWriter writer );

        public abstract int GetSubInfoSize();
    }
}
