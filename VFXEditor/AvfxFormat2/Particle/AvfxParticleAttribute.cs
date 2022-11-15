using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxParticleAttribute : AvfxAssignable {
        public readonly AvfxParticle Particle;
        public readonly List<UiNodeSelect> NodeSelects = new();
        public readonly List<AvfxItem> DisplayTabs;
        public readonly UiDisplayList Display;

        public AvfxParticleAttribute( string avfxName, AvfxParticle particle) : base( avfxName ) {
            Particle = particle;
            DisplayTabs = new List<AvfxItem> {
                ( Display = new UiDisplayList( "Parameters" ) )
            };
        }

        protected void InitNodeSelects() {
            NodeSelects.AddRange( GetNodeSelects() );
            if( !IsAssigned() ) NodeSelects.ForEach( x => x.Disable() );
            NodeSelects.ForEach( Display.Add );
        }

        protected void EnableAllSelectors() => NodeSelects.ForEach( x => x.Enable() );

        protected void DisableAllSelectors() => NodeSelects.ForEach( x => x.Enable() );

        protected void Assign() => CommandManager.Avfx.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, true ) );

        protected void Unassign() => CommandManager.Avfx.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, false ) );

        public abstract List<UiNodeSelect> GetNodeSelects();
    }
}
