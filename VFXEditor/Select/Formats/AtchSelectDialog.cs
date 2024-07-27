using VfxEditor.Formats.AtchFormat;
using VfxEditor.Select.Tabs.Character;

namespace VfxEditor.Select.Formats {
    public class AtchSelectDialog : SelectDialog {
        public AtchSelectDialog( string id, AtchManager manager, bool isSourceDialog ) : base( id, "atch", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new CharacterTabAtch( this, "Character" )
            ] );
        }
    }
}