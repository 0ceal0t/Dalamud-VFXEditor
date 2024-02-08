using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListMoveCommand<T> : ICommand where T : class {
        protected readonly Action<T> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected readonly T Destination;
        protected readonly int Idx;
        protected readonly int NewIdx;

        public ListMoveCommand( List<T> items, T item, T destination, Action<T> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
            Destination = destination;
            Idx = Items.IndexOf( Item );
            NewIdx = Items.IndexOf( Destination );

            Items.Remove( Item );
            Items.Insert( NewIdx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Redo() {
            Items.Remove( Item );
            Items.Insert( NewIdx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Undo() {
            Items.Remove( Item );
            Items.Insert( Idx, Item );
            OnChangeAction?.Invoke( Item );
        }
    }
}
