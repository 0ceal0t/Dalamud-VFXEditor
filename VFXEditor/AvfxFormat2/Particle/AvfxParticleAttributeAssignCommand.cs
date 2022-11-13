using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleAttributeAssignCommand : ICommand {
        private readonly AvfxParticleAttribute Item;
        private readonly List<UiNodeSelect> NodeSelects;
        private readonly bool State;

        public AvfxParticleAttributeAssignCommand( AvfxParticleAttribute item, List<UiNodeSelect> nodeSelects, bool state ) {
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
