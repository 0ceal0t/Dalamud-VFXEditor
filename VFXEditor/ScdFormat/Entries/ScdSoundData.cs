using NAudio.Wave;

namespace VfxEditor.ScdFormat {
    public abstract class ScdSoundData {
        public abstract WaveStream GetStream();
    }
}
