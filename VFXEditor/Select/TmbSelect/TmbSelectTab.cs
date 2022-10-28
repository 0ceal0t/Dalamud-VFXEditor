using VFXEditor.Select.Sheets;

namespace VFXEditor.Select.TmbSelect {
    public abstract class TmbSelectTab<T, S> : SelectTab<T, S> where T : class where S : class {
        protected readonly TmbSelectDialog Dialog;

        public TmbSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, TmbSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }
    }
}