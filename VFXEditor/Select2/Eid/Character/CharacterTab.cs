namespace VfxEditor.Select2.Eid.Character {
    public class CharacterTab : SelectTab<CharacterRow> {
        public CharacterTab( SelectDialog dialog, string name ) : base( dialog, name, "Eid-Character" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) {
                Items.Add( new( item.Key, item.Value.SkeletonId ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameNpc, Selected.Name );
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
