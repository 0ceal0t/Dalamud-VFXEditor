namespace VfxEditor.AvfxFormat {
    public class AvfxStringCommand : ICommand {
        private readonly AvfxString Item;
        private readonly bool Unassign;
        private readonly bool PrevAssign;
        private readonly string State;
        private readonly string PrevState;

        public AvfxStringCommand( AvfxString item, string state, bool unassign ) {
            Item = item;
            Unassign = unassign;
            PrevAssign = item.IsAssigned();
            State = state;
            PrevState = item.GetValue();
        }

        public void Execute() {
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
