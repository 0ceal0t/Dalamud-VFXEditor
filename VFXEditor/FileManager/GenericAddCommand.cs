using System;
using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericAddCommand<T> : ICommand where T : class {
        protected readonly Action<T> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected int Idx;

        public GenericAddCommand( List<T> items, T item, Action<T> onChangeAction = null ) : this( items, item, -1, onChangeAction ) { }

        public GenericAddCommand( List<T> items, T item, int idx, Action<T> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
            Idx = idx;
        }

        public virtual void Execute() {
            Idx = Idx == -1 ? Items.Count : Idx;
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Redo() {
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Undo() {
            Items.Remove( Item );
            OnChangeAction?.Invoke( Item );
        }
    }
}
