using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Pap.Weapon {
    public class WeaponTab : SelectTab<WeaponRow, string> {
        public WeaponTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-Weapon", SelectResultType.GameItem ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var weapon = new WeaponRow( row );
                    if( weapon.HasModel ) Items.Add( weapon );
                }
            }
        }

        public override void LoadSelection( WeaponRow item, out string loaded ) {
            loaded = item.GetPapPath();
            if( !Dalamud.DataManager.FileExists( loaded ) ) loaded = "";
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            if( string.IsNullOrEmpty( Loaded ) ) return;
            DrawPath( "Animations", Loaded, Selected.Name );
        }

        protected override string GetName( WeaponRow item ) => item.Name;
    }
}
