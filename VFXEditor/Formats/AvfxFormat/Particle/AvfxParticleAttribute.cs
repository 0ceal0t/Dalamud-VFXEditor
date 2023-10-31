using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxParticleAttribute : AvfxOptional {
        public readonly AvfxParticle Particle;
        public readonly List<AvfxNodeSelect> NodeSelects = new();
        public readonly List<AvfxItem> DisplayTabs;
        public readonly UiDisplayList Display;

        public AvfxParticleAttribute( string avfxName, AvfxParticle particle ) : base( avfxName ) {
            Particle = particle;
            DisplayTabs = new() {
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

        protected void Assign() => CommandManager.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, true ) );

        protected void Unassign() => CommandManager.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, false ) );

        public abstract List<AvfxNodeSelect> GetNodeSelects();
    }
}
