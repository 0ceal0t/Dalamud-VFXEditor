namespace VfxEditor.Select.Shared.Item {
    public abstract class ItemTab<T> : SelectTab<ItemRow, T> where T : class {
        public ItemTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Item", SelectResultType.GameItem ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var row in Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if( row.EquipSlotCategory.Value?.MainHand == 1 || row.EquipSlotCategory.Value?.OffHand == 1 ) {
                    var weapon = new WeaponRow( row );
                    if( weapon.HasModel ) Items.Add( weapon );
                    if( weapon.HasSubModel ) Items.Add( weapon.SubItem );
                }
                else if(
                    row.EquipSlotCategory.Value?.Head == 1 ||
                    row.EquipSlotCategory.Value?.Body == 1 ||
                    row.EquipSlotCategory.Value?.Gloves == 1 ||
                    row.EquipSlotCategory.Value?.Legs == 1 ||
                    row.EquipSlotCategory.Value?.Feet == 1
                ) {
                    var armor = new ArmorRow( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        protected override string GetName( ItemRow item ) => item.Name;
    }
}
