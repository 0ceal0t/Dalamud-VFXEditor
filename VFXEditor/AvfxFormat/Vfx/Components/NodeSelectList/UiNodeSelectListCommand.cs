using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeSelectListCommand<T> : ICommand where T : UiNode {
        private readonly UiNodeSelectList<T> Item;
        private readonly int Idx;
        private readonly T State;
        private readonly T PrevState;

        public UiNodeSelectListCommand( UiNodeSelectList<T> item, T state, int idx ) {
            Item = item;
            State = state;
            Idx = idx;
            PrevState = item.Selected[Idx];
        }

        public void Execute() {
            Item.Select( State, Idx );
        }

        public void Redo() {
            Item.Select( State, Idx );
        }

        public void Undo() {
            Item.Select( PrevState, Idx );
        }
    }
}
