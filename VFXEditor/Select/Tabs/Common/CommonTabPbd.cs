namespace VfxEditor.Select.Tabs.Common {
    public class CommonTabPbd : SelectTab<CommonRow> {
        public CommonTabPbd( SelectDialog dialog, string name ) : base( dialog, name, "Common-Pbd" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            Items.Add( new CommonRow( 0, "chara/xls/boneDeformer/human.pbd", "Human", 0 ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameUi );
        }
    }
}
