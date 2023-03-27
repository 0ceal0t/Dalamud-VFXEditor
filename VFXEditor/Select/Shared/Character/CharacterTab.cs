namespace VfxEditor.Select.Shared.Character {
    public abstract class CharacterTab : SelectTab<CharacterRow> {
        public CharacterTab( SelectDialog dialog, string name ) : base( dialog, name, "Character-Shared" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value.SkeletonId ) );
        }

        // ===== DRAWING ======

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
