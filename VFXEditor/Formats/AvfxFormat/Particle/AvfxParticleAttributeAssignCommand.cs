using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleAttributeAssignCommand : ICommand {
        private readonly AvfxParticleAttribute Item;
        private readonly List<AvfxNodeSelect> NodeSelects;
        private readonly bool State;

        public AvfxParticleAttributeAssignCommand( AvfxParticleAttribute item, List<AvfxNodeSelect> nodeSelects, bool state ) {
            Item = item;
            NodeSelects = nodeSelects;
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
            if( state ) { // set assigned
                AvfxBase.RecurseAssigned( Item, state );
                NodeSelects.ForEach( x => x.Enable() );
            }
            else { // set unassigned
                Item.SetAssigned( state );
                NodeSelects.ForEach( x => x.Disable() );
            }
        }
    }
}
