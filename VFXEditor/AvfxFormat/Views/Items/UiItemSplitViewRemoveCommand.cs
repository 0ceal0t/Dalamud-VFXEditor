using System;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiItemSplitViewRemoveCommand<T> : ICommand where T : UiItem {
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
            View.RemoveFromAvfx( Item );
            View.UpdateIdx();
            View.ClearSelected();
        }

        public void Undo() {
            Group.Insert( Idx, Item );
            View.AddToAvfx( Item, Idx );
            View.UpdateIdx();
            View.ClearSelected();
        }
    }
}
