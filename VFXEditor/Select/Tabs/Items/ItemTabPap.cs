namespace VfxEditor.Select.Tabs.Items {
    public class ItemTabPap : ItemTab<string> {
        public ItemTabPap( SelectDialog dialog, string name ) : base( dialog, name, "Item-Pap", ItemTabFilter.Weapon ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out string loaded ) {
            loaded = "";
            if( item is not ItemRowWeapon weapon ) return;

            loaded = weapon.PapPath;
            if( !Dalamud.DataManager.FileExists( loaded ) ) loaded = "";
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );

            DrawPaths( Loaded, Selected.Name );
        }
    }
}