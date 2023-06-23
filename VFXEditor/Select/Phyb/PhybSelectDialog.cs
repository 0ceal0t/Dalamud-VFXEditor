using VfxEditor.FileManager;
using VfxEditor.Select.Phyb.Armor;
using VfxEditor.Select.Phyb.Character;
using VfxEditor.Select.Phyb.Npc;

namespace VfxEditor.Select.Phyb {
    public class PhybSelectDialog : SelectDialog {
        public PhybSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "phyb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new ArmorTab( this, "Armor" ),
                new NpcPhybTab( this, "Npc" ),
                new CharacterPhybTab( this, "Character" )
            } );
        }
    }
}
