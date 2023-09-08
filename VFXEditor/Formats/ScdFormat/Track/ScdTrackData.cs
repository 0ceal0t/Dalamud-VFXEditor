using System.IO;

namespace VfxEditor.ScdFormat {
    public abstract class ScdTrackData {
        public abstract void Read( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );

        public abstract void Draw();
    }
}
