using System.IO;
using VfxEditor.Parsing.Data;

namespace VfxEditor.ScdFormat {
    public abstract class ScdTrackData : IData {
        public abstract void Read( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );

        public abstract void Draw();

        public void Enable() { }
        public void Disable() { }
    }
}
