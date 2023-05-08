using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Pap.Weapon {
    public class WeaponTab : SelectTab<WeaponRow, string> {
        public WeaponTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-Weapon" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var weapon = new WeaponRow( row );
                    if( weapon.HasModel ) Items.Add( weapon );
                }
            }
        }

        public override void LoadSelection( WeaponRow item, out string loaded ) {
            loaded = item.GetPapPath();
            if( !Plugin.DataManager.FileExists( loaded ) ) loaded = "";
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            if( string.IsNullOrEmpty( Loaded ) ) return;

            Dialog.DrawPath( "Animations", Loaded, parentId, SelectResultType.GameItem, Selected.Name );
        }

        protected override string GetName( WeaponRow item ) => item.Name;
    }
}
