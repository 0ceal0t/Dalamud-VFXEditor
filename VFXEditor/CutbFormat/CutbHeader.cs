using VfxEditor.Ui.Interfaces;

namespace VfxEditor.CutbFormat {
    public abstract class CutbHeader : IUiItem {
        public abstract string Magic { get; }

        public abstract void Draw();
    }
}
