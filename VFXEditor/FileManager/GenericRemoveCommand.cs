using System;
using System.Collections.Generic;

namespace VfxEditor.FileManager {
    public class GenericRemoveCommand<T> : ICommand where T : class {
        protected readonly List<T> Group;
        protected readonly T Item;
        protected readonly int Idx;

        public GenericRemoveCommand( List<T> group, T item ) {
            Group = group;
            Item = item;
            Idx = Group.IndexOf( item );
        }

        public virtual void Execute() => Group.Remove( Item );

        public virtual void Redo() => Group.Remove( Item );

        public virtual void Undo() => Group.Insert( Idx, Item );
    }
}
