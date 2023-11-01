using VfxEditor.Formats.AtchFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Npc;

namespace VfxEditor.Select.Formats {
    public class AtchSelectDialog : SelectDialog {
        public AtchSelectDialog( string id, AtchManager manager, bool isSourceDialog ) : base( id, "atch", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new NpcTabAtch( this, "Npc" ),
                new CharacterTabAtch( this, "Character" ),
            } );
        }
    }
}