namespace VfxEditor.AvfxFormat {
    public class AvfxStringCommand : ICommand {
        private readonly AvfxString Item;
        private readonly string State;
        private readonly bool Unassign;
        private bool PrevAssign;
        private string PrevState;

        public AvfxStringCommand( AvfxString item, string state, bool unassign ) {
            Item = item;
            State = state;
            Unassign = unassign;
        }

        public void Execute() {
            PrevAssign = Item.IsAssigned();
            PrevState = Item.GetValue();
            Item.SetValue( State );
            if( Unassign ) Item.SetAssigned( false );
        }

        public void Redo() {
            Item.SetValue( State );
            if( Unassign ) Item.SetAssigned( false );
        }

        public void Undo() {
            Item.SetValue( PrevState );
            Item.SetAssigned( PrevAssign );
        }
    }
}
