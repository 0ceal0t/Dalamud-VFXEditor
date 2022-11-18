using System;
using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericAddCommand<T> : ICommand where T : class {
        protected readonly List<T> Group;
        protected readonly T Item;
        protected readonly int Idx;

        public GenericAddCommand( List<T> group, T item ) : this( group, item, group.Count ) { }

        public GenericAddCommand( List<T> group, T item, int idx ) {
            Group = group;
            Item = item;
            Idx = idx;
        }

        public virtual void Execute() => Group.Insert( Idx, Item );

        public virtual void Redo() => Group.Insert( Idx, Item );

        public virtual void Undo() => Group.Remove( Item );
    }
}
