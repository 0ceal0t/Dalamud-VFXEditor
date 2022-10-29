using VfxEditor.Select.Sheets;

namespace VfxEditor.Select.VfxSelect {
    public abstract class VfxSelectTab<T, S> : SelectTab<T, S> where T : class where S : class {
        protected readonly VfxSelectDialog Dialog;

        public VfxSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, VfxSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }
    }
}