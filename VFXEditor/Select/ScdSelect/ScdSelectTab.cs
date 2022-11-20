using VfxEditor.Select.Sheets;

namespace VfxEditor.Select.ScdSelect {
    public abstract class ScdSelectTab<T, S> : SelectTab<T, S> where T : class where S : class {
        protected readonly ScdSelectDialog Dialog;

        public ScdSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, ScdSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }
    }
}