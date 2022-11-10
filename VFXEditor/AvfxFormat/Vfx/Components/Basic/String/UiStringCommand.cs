using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiStringCommand : ICommand {
        private readonly UiString Item;
        private readonly bool Unassign;
        private readonly bool PrevAssign;
        private readonly string State;
        private readonly string PrevState;

        public UiStringCommand( UiString item, string state, bool unassign ) {
            Item = item;
            Unassign = unassign;
            PrevAssign = item.Literal.IsAssigned();
            State = state;
            PrevState = item.Literal.GetValue();
        }

        public void Execute() {
            Item.InputString = State.Trim( '\0' );
            Item.Literal.SetValue( State );
            if( Unassign ) Item.Literal.SetAssigned( false );
        }

        public void Redo() {
            Item.InputString = State.Trim( '\0' );
            Item.Literal.SetValue( State );
            if( Unassign ) Item.Literal.SetAssigned( false );
        }

        public void Undo() {
            Item.InputString = PrevState.Trim( '\0' );
            Item.Literal.SetValue( PrevState );
            Item.Literal.SetAssigned( PrevAssign );
        }
    }
}
