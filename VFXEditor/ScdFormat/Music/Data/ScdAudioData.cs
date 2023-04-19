using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat.Music.Data {
    public abstract class ScdAudioData {
        public abstract WaveStream GetStream();

        public abstract int SamplesToBytes( int samples );

        public abstract int TimeToBytes( float time );

        public abstract void BytesToLoopStartEnd( int loopStart, int loopEnd, out double startTime, out double endTime );

        public abstract void Write( BinaryWriter writer );
    }
}
