using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListAddCommand<T> : ICommand where T : class {
        protected readonly Action<T, bool> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected int Idx;

        public ListAddCommand( List<T> items, T item, Action<T, bool> onChangeAction = null ) : this( items, item, -1, onChangeAction ) { }

        public ListAddCommand( List<T> items, T item, int idx, Action<T, bool> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
            Idx = idx;
        }

        public virtual void Execute() {
            Idx = Idx == -1 ? Items.Count : Idx;
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
