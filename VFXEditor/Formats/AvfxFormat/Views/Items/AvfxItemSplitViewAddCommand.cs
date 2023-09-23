using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxItemSplitViewAddCommand<T> : ICommand where T : class, IIndexUiItem {
        private readonly AvfxItemSplitView<T> View;
        private readonly List<T> Group;
        private int Idx;
        private T Item;

        public AvfxItemSplitViewAddCommand( AvfxItemSplitView<T> view, List<T> group ) {
            View = view;
            Group = group;
        }

        public void Execute() {
            Idx = Group.Count;
            Item = View.CreateNewAvfx();
            Add();
        }

        public void Redo() => Add();

        public void Undo() {
            if( Item == null ) return;
            Group.Remove( Item );
            View.Disable( Item );
            View.UpdateIdx();
            View.ClearSelected();
        }

        private void Add() {
            if( Item == null ) return;
            Group.Insert( Idx, Item );
            View.Enable( Item );
            View.UpdateIdx();
        }
    }
}
