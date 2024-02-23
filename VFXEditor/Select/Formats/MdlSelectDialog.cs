using VfxEditor.Formats.MdlFormat;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Items;

namespace VfxEditor.Select.Formats {
    public class MdlSelectDialog : SelectDialog {
        public MdlSelectDialog( string id, MdlManager manager, bool isSourceDialog ) : base( id, "mdl", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new ItemTabMdl( this, "Item" ),
                new CharacterTabMdl( this, "Character" ),
            ] );
        }
    }
}
