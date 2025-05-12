using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxParticleAttribute : AvfxOptional {
        public readonly AvfxParticle Particle;
        public readonly List<AvfxNodeSelect> NodeSelects = [];
        public readonly List<AvfxItem> DisplayTabs;
        public readonly UiDisplayList Display;

        public AvfxParticleAttribute( string avfxName, AvfxParticle particle, bool locked = false ) : base( avfxName, locked ) {
            Particle = particle;
            DisplayTabs = [
                ( Display = new UiDisplayList( "Parameters" ) )
            ];
        }

        protected void InitNodeSelects() {
            NodeSelects.AddRange( GetNodeSelects() );
            if( !IsAssigned() ) NodeSelects.ForEach( x => x.Disable() );
            NodeSelects.ForEach( Display.Add );
        }

        protected void EnableAllSelectors() => NodeSelects.ForEach( x => x.Enable() );
    }
}
