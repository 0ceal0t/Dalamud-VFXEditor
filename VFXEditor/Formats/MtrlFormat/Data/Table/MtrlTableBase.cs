using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public enum ColorTableSize : int {
        Legacy = 16 * 32,
        Extended = 32 * 64
    }

    public enum DyeTableSize : int {
        Legacy = 16 * 2,
        Extended = 32 * 4
    }

    public abstract class MtrlTableBase {
        public readonly MtrlFile File;

        public MtrlTableBase( MtrlFile file ) {
            File = file;
        }

        public abstract void Write( BinaryWriter writer );

        public abstract void Draw();
    }
}
