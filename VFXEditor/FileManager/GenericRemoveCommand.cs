using System;
using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericRemoveCommand<T> : ICommand where T : class {
        protected readonly Action<T> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected int Idx;

        public GenericRemoveCommand( List<T> items, T item, Action<T> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
        }

        public virtual void Execute() {
            Idx = Items.IndexOf( Item );
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Redo() {
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Undo() {
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item );
        }
    }
}
