using System.Collections.Generic;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Assign {
    public class AvfxAssignCommand : ICommand {
        private readonly AvfxBase Item;
        private readonly bool State;
        private readonly bool Recurse;
        private readonly bool ToggleState;
        private readonly bool PrevState;
        private readonly IEnumerable<AvfxNodeSelect> NodeSelects;

        public AvfxAssignCommand( AvfxBase item, bool assigned, bool recurse = false, bool toggleState = false ) {
            Item = item;
            State = assigned;
            Recurse = recurse;
            ToggleState = toggleState;
            PrevState = ToggleState ? !State : Item.IsAssigned();
            NodeSelects = item.GetNodeSelects();

            SetState( State );
        }

        public void Redo() => SetState( State );

        public void Undo() => SetState( PrevState );

        private void SetState( bool state ) {
            Item.SetAssigned( state, Recurse );

            foreach( var select in NodeSelects ) {
                if( state ) select.Enable();
                else select.Disable();
            }
        }

    }
}
