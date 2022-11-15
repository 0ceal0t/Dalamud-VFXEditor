using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat2 {
    public class UiItemSplitViewRemoveCommand<T> : ICommand where T : class, IUiSelectableItem {
        private readonly UiItemSplitView<T> View;
        private readonly List<T> Group;
        private readonly T Item;
        private readonly int Idx;

        public UiItemSplitViewRemoveCommand( UiItemSplitView<T> view, List<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
            Idx = Group.IndexOf( item );
        }

        public void Execute() => Redo();

        public void Redo() {
            Group.Remove( Item );
            View.Disable( Item );
            View.UpdateIdx();
            View.ClearSelected();
        }

        public void Undo() {
            Group.Insert( Idx, Item );
            View.Enable( Item );
            View.UpdateIdx();
        }
    }
}
