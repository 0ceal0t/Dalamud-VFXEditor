namespace VfxEditor.Select.Shared.Character {
    public abstract class CharacterTab : SelectTab<CharacterRow> {
        public CharacterTab( SelectDialog dialog, string name ) : base( dialog, name, "Character-Shared", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectDataUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value ) );
        }

        // ===== DRAWING ======

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
