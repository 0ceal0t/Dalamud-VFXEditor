using System.IO;

namespace VfxEditor.Ui.Interfaces {
    public interface IWriteableIUiItem : IUiItem {
        public void Write( BinaryWriter writer );
    }
}
