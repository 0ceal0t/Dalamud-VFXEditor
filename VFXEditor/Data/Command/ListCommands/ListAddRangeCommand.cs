using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListAddRangeCommand<T> : ICommand where T : class {
        protected readonly Action<List<T>, bool> OnChangeAction;
        protected readonly List<T> Items;
        protected readonly List<T> NewItems;
        protected readonly int Idx;

        public ListAddRangeCommand( List<T> items, List<T> newItems, Action<List<T>, bool> onChangeAction = null ) : this( items, newItems, items.Count, onChangeAction ) { }

        public ListAddRangeCommand( List<T> items, List<T> newItems, int idx, Action<List<T>, bool> onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            NewItems = newItems;
            Idx = idx;

            Items.InsertRange( Idx, NewItems );
            OnChangeAction?.Invoke( NewItems, true );
        }

        public virtual void Redo() {
            Items.InsertRange( Idx, NewItems );
            OnChangeAction?.Invoke( NewItems, true );
        }

        public virtual void Undo() {
            Items.RemoveAll( NewItems.Contains );
            OnChangeAction?.Invoke( NewItems, false );
        }
    }
}
