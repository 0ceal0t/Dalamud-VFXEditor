using VfxEditor.Formats.AtchFormat;
using VfxEditor.Select.Atch.Character;
using VfxEditor.Select.Atch.Npc;

namespace VfxEditor.Select.Atch {
    public class AtchSelectDialog : SelectDialog {
        public AtchSelectDialog( string id, AtchManager manager, bool isSourceDialog ) : base( id, "atch", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new NpcAtchTab( this, "Npc" ),
                new CharacterAtchTab( this, "Character" ),
            } );
        }
    }
}
