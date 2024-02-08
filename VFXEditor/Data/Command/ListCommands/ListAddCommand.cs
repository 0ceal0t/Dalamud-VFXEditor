using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListAddCommand<T> : ICommand where T : class {
        protected readonly Action<T, bool> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected readonly int Idx;

        public ListAddCommand( List<T> items, T item, Action<T, bool> onChangeAction = null ) : this( items, item, items.Count, onChangeAction ) { }

        public ListAddCommand( List<T> items, T item, int idx, Action<T, bool> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
            Idx = idx;

            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item, true );
        }

        public virtual void Redo() {
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item, true );
        }

        public virtual void Undo() {
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item, false );
        }
    }
}
