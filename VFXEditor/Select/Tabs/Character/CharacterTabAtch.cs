namespace VfxEditor.Select.Tabs.Character {
    public class CharacterTabAtch : CharacterTab {
        public CharacterTabAtch( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void DrawSelected() {
            DrawPaths( Selected.AtchPath, Selected.Name );
        }
    }
}