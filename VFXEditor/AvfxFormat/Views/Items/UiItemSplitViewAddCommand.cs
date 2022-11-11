using System;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiItemSplitViewAddCommand<T> : ICommand where T : UiItem {
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
            View.RemoveFromAvfx( Item );
            View.UpdateIdx();
            View.ClearSelected();
        }

        private void Add() {
            if( Item == null ) return;
            Group.Insert( Idx, Item );
            View.AddToAvfx( Item, Idx );
            View.UpdateIdx();
        }
    }
}
