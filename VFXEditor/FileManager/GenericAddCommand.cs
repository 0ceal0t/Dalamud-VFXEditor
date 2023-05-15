using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericAddCommand<T> : ICommand where T : class {
        protected readonly List<T> Group;
        protected readonly T Item;
        protected int Idx;

        public GenericAddCommand( List<T> group, T item ) : this( group, item, -1 ) { }

        public GenericAddCommand( List<T> group, T item, int idx ) {
            Group = group;
            Item = item;
            Idx = idx;
        }

        public virtual void Execute() {
            Idx = Idx == -1 ? Group.Count : Idx;
            Group.Insert( Idx, Item );
        }

        public virtual void Redo() => Group.Insert( Idx, Item );

        public virtual void Undo() => Group.Remove( Item );
    }
}
