using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat {
    public abstract class ScdAudioData {
        public abstract WaveStream GetStream();

        public abstract void Write( BinaryWriter writer );
    }
}
