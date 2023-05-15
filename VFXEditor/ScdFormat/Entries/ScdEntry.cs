using System.IO;

namespace VfxEditor.ScdFormat {
    public abstract class ScdEntry {
        public abstract void Read( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );

        public void Read( BinaryReader reader, int offset ) {
            var oldPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            Read( reader );
            reader.BaseStream.Position = oldPosition;
        }
    }
}
