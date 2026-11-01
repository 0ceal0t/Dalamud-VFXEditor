using System.Collections.Generic;
using VfxEditor.Data.Command;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AvfxFormat.Views.Items {
    public class AvfxItemSplitViewRemoveCommand<T> : ICommand where T : class, IIndexUiItem {
        private readonly AvfxItemSplitView<T> View;
        private readonly List<T> Group;
        private readonly T Item;
        private readonly int Idx;

        public AvfxItemSplitViewRemoveCommand( AvfxItemSplitView<T> view, List<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
            Idx = Group.IndexOf( Item );

            Redo();
        }

        public void Redo() {
            Group.Remove( Item );
            View.Disable( Item );
            View.UpdateIdx();
            View.ClearSelected();
            View.OnChange();
        }

        public void Undo() {
            Group.Insert( Idx, Item );
            View.Enable( Item );
            View.UpdateIdx();
            View.OnChange();
        }
    }
}
