using VFXEditor.Select.Sheets;

namespace VFXEditor.Select.VFX {
    public abstract class VFXSelectTab<T, S> : SelectTab<T, S> where T : class where S : class {
        protected readonly VFXSelectDialog Dialog;

        public VFXSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, VFXSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }
    }
}