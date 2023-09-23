using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxItemSplitViewRemoveCommand<T> : ICommand where T : class, IIndexUiItem {
        private readonly AvfxItemSplitView<T> View;
        private readonly List<T> Group;
        private readonly T Item;
        private int Idx;

        public AvfxItemSplitViewRemoveCommand( AvfxItemSplitView<T> view, List<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
        }

        public void Execute() {
            Idx = Group.IndexOf( Item );
            Redo();
        }

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
