using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.EidFormat.BindPoint {
    public abstract class EidBindPoint : IUiItem {
        public abstract int GetId();

        public abstract void Draw();

        public abstract void Write( BinaryWriter writer );
    }
}
