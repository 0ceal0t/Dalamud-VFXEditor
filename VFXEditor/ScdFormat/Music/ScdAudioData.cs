using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat {
    public abstract class ScdAudioData {
        public abstract WaveStream GetStream();

        public abstract int SamplesToBytes( int samples );

        public abstract int TimeToBytes( float time );

        public abstract void Write( BinaryWriter writer );
    }
}
