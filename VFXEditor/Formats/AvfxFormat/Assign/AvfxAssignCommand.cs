using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Assign {
    public class AvfxAssignCommand : ICommand {
        private readonly AvfxBase Item;
        private readonly bool Assigned;
        private readonly bool Recurse;
        private readonly bool ToggleState;
        private readonly bool PrevState;

        public AvfxAssignCommand( AvfxBase item, bool assigned, bool recurse, bool toggleState ) {
            Item = item;
            Assigned = assigned;
            Recurse = recurse;
            ToggleState = toggleState;
            PrevState = ToggleState ? !Assigned : Item.IsAssigned();

            Item.SetAssigned( Assigned, Recurse );
        }

        public void Redo() => Item.SetAssigned( Assigned, Recurse );

        public void Undo() => Item.SetAssigned( PrevState, Recurse );

    }
}
