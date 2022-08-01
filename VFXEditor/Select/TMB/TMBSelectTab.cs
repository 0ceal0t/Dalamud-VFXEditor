using VFXEditor.Select.Sheets;

namespace VFXEditor.Select.TMB {
    public abstract class TMBSelectTab<T, S> : SelectTab<T, S> {
        protected readonly TMBSelectDialog Dialog;

        public TMBSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, TMBSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }
    }
}