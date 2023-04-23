using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Eid.Character {
    public class CharacterEidTab : CharacterTab {
        public CharacterEidTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.EidPath, parentId, SelectResultType.GameCharacter, Selected.Name );
        }
    }
}
