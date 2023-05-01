namespace VfxEditor.AvfxFormat {
    public class AvfxAssignCommand : ICommand {
        private readonly AvfxBase Item;
        private readonly bool State;
        private bool PrevState;

        public AvfxAssignCommand( AvfxBase item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.IsAssigned();
            Item.SetAssigned( State );
        }

        public void Redo() => Item.SetAssigned( State );

        public void Undo() => Item.SetAssigned( PrevState );
    }
}
