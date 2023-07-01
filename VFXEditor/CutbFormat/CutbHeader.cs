using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.CutbFormat {
    public abstract class CutbHeader : ParsedData, IUiItem {
        public abstract string Magic { get; }

        public abstract void Draw();
    }
}
