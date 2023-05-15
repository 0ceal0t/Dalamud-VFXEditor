using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Eid.Character {
    public class CharacterEidTab : CharacterTab {
        public CharacterEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            Dialog.DrawPath( "Path", Selected.EidPath, SelectResultType.GameCharacter, Selected.Name );
        }
    }
}
