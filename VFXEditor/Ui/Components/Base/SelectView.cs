using System.Collections.Generic;

namespace VfxEditor.Ui.Components.Base {
    public class SelectView<T> where T : class {
        public readonly string Id;
        public readonly List<T> Items;

        protected T Selected = null;

        public SelectView( string id, List<T> items ) {
            Id = id;
            Items = items;
        }

        public T GetSelected() => Selected;

        public virtual void SetSelected( T selected ) {
            Selected = selected;
        }

        public virtual void ClearSelected() {
            Selected = null;
        }

        public virtual string GetText( T item, int idx ) => $"{Id} {idx}";
    }
}
