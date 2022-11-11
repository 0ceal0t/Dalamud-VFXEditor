using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiTextureItem : UiAssignableItem {
        public readonly UiParticle Particle;

        public readonly List<UiNodeSelect> NodeSelects = new();
        public readonly List<UiItem> Tabs;
        public readonly UiParameters Parameters;

        public UiTextureItem( UiParticle particle ) {
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) )
            };
        }

        protected void InitNodeSelects() {
            NodeSelects.AddRange( GetNodeSelects() );
            if( !IsAssigned() ) NodeSelects.ForEach( x => x.Disable() );
            NodeSelects.ForEach( Parameters.Add );
        }

        protected void Assign( AVFXBase item ) => CommandManager.Avfx.Add( new UiTextureItemAssignCommand( item, NodeSelects, true ) );

        protected void Unassign( AVFXBase item ) => CommandManager.Avfx.Add( new UiTextureItemAssignCommand( item, NodeSelects, false ) );

        public abstract List<UiNodeSelect> GetNodeSelects();
    }
}
