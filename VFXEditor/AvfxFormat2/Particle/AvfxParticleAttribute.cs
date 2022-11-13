using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxParticleAttribute : AvfxAssignable {
        public readonly AvfxParticle Particle;
        public readonly List<UiNodeSelect> NodeSelects = new();
        public readonly List<AvfxItem> Tabs;
        public readonly UiParameters Parameters;

        public AvfxParticleAttribute( string avfxName, AvfxParticle particle) : base( avfxName ) {
            Particle = particle;
            Tabs = new List<AvfxItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };
        }

        protected void InitNodeSelects() {
            NodeSelects.AddRange( GetNodeSelects() );
            if( !IsAssigned() ) NodeSelects.ForEach( x => x.Disable() );
            NodeSelects.ForEach( Parameters.Add );
        }

        protected void EnableAllSelectors() => NodeSelects.ForEach( x => x.Enable() );

        protected void DisableAllSelectors() => NodeSelects.ForEach( x => x.Enable() );

        protected void Assign() => CommandManager.Avfx.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, true ) );

        protected void Unassign() => CommandManager.Avfx.Add( new AvfxParticleAttributeAssignCommand( this, NodeSelects, false ) );

        public abstract List<UiNodeSelect> GetNodeSelects();
    }
}
