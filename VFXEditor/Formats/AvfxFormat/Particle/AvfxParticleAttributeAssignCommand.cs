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

            SetState( State );
        }

        public void Redo() => SetState( State );

        public void Undo() => SetState( !State );

        private void SetState( bool state ) {
            Item.SetAssigned( state, true );

            if( state ) NodeSelects.ForEach( x => x.Enable() );
            else NodeSelects.ForEach( x => x.Disable() );
        }
    }
}
