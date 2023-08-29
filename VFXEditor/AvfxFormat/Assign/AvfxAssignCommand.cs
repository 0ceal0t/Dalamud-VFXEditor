namespace VfxEditor.AvfxFormat {
    public class AvfxAssignCommand : ICommand {
        private readonly AvfxBase Item;
        private readonly bool Assigned;
        private bool PrevState;

        public AvfxAssignCommand( AvfxBase item, bool assigned ) {
            Item = item;
            Assigned = assigned;
        }

        public void Execute() {
            PrevState = Item.IsAssigned();
            Item.SetAssigned( Assigned );
        }

        public void Redo() => Item.SetAssigned( Assigned );

        public void Undo() => Item.SetAssigned( PrevState );
    }
}
