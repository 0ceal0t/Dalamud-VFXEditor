using System;
using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericMoveCommand<T> : ICommand where T : class {
        protected readonly Action<T> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly T Item;
        protected readonly T Destination;
        protected int ItemIdx;
        protected int DestinationIdx;

        public GenericMoveCommand( List<T> items, T item, T destination, Action<T> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            Item = item;
            Destination = destination;
        }

        public virtual void Execute() {
            ItemIdx = Items.IndexOf( Item );
            DestinationIdx = Items.IndexOf( Destination );

            Items.Remove( Item );
            Items.Insert( DestinationIdx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Redo() {
            Items.Remove( Item );
            Items.Insert( DestinationIdx, Item );
            OnChangeAction?.Invoke( Item );
        }

        public virtual void Undo() {
            Items.Remove( Item );
            Items.Insert( ItemIdx, Item );
            OnChangeAction?.Invoke( Item );
        }
    }
}
