using VfxEditor.FileManager;
using VfxEditor.Select.Eid.Character;
using VfxEditor.Select.Eid.Mount;
using VfxEditor.Select.Eid.Npc;

namespace VfxEditor.Select.Eid {
    public class EidSelectDialog : SelectDialog {
        public EidSelectDialog( string id, FileManagerBase manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new NpcEidTab( this, "Npc" ),
                new CharacterEidTab( this, "Character" ),
                new MountEidTab( this, "Mount" ),
            } );
        }
    }
}
