using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListRemoveCommand<T> : ICommand where T : class {
        protected readonly Action<T, bool> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected int Idx;

        public ListRemoveCommand( List<T> items, T item, Action<T, bool> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
        }

        public virtual void Execute() {
            Idx = Items.IndexOf( Item );
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item, true );
        }

        public virtual void Redo() {
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item, true );
        }

        public virtual void Undo() {
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item, false );
        }
    }
}
