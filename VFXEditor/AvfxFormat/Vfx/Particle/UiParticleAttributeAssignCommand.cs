using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleAttributeAssignCommand : ICommand {
        private readonly AVFXBase Item;
        private readonly List<UiNodeSelect> NodeSelects;
        private readonly bool State;

        public UiParticleAttributeAssignCommand( AVFXBase item, List<UiNodeSelect> nodeSelects, bool state ) {
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
            if( state ) {
                AVFXBase.RecurseAssigned( Item, state );
                NodeSelects.ForEach( x => x.Enable() );
            }
            else {
                Item.SetAssigned( state );
                NodeSelects.ForEach( x => x.Disable() );
            }
        }
    }
}
