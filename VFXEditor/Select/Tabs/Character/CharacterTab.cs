using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public abstract class CharacterTab : SelectTab<CharacterRow> {
        public CharacterTab( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        public override void LoadData() => Load( Items );

        protected override string GetName( CharacterRow item ) => item.Name;

        // =====================

        public static void Load( List<CharacterRow> items ) {
            items.AddRange( SelectDataUtils.CharacterRaces.Select( x => new CharacterRow( x ) ) );
        }
    }
}