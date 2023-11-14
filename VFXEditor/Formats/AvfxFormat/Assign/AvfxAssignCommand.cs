using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Assign {
    public class AvfxAssignCommand : ICommand {
        private readonly AvfxBase Item;
        private readonly bool Assigned;
        private readonly bool Recurse;
        private readonly bool Toggle;
        private bool PrevState;

        public AvfxAssignCommand( AvfxBase item, bool assigned, bool recurse, bool toggle ) {
            Item = item;
            Assigned = assigned;
            Recurse = recurse;
            Toggle = toggle;
        }

        public void Execute() {
            PrevState = Toggle ? !Assigned : Item.IsAssigned();
            Item.SetAssigned( Assigned, Recurse );
        }

        public void Redo() => Item.SetAssigned( Assigned, Recurse );

        public void Undo() => Item.SetAssigned( PrevState, Recurse );

    }
}
