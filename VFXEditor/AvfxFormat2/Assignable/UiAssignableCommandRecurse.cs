using System;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    internal class UiAssignableCommandRecurse : ICommand {
        private readonly AvfxBase Item;
        private readonly bool State;

        public UiAssignableCommandRecurse( AvfxBase item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            SetState( State );
        }

        public void Redo() {
            SetState( State );
        }

        public void Undo() {
            SetState( !State );
        }

        private void SetState( bool state ) {
            if( state ) AvfxBase.RecurseAssigned( Item, state ); // true
            else Item.SetAssigned( state ); // false
        }
    }
}
