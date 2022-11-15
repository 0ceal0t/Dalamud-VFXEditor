using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat2 {
    public class UiItemSplitViewAddCommand<T> : ICommand where T : class, IUiSelectableItem {
        private readonly UiItemSplitView<T> View;
        private readonly List<T> Group;
        private readonly int Idx;
        private T Item;

        public UiItemSplitViewAddCommand( UiItemSplitView<T> view, List<T> group ) {
            View = view;
            Group = group;
            Idx = Group.Count;
        }

        public void Execute() {
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
