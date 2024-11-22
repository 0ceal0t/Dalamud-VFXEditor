using System;

namespace VfxEditor.Select.Tabs.Items {
    [Flags]
    public enum ItemTabFilter {
        Weapon = 0x01,
        SubWeapon = 0x02,
        Armor = 0x04,
        Accessory = 0x08,
        Glasses = 0x10
    }

    public abstract class ItemTab<T> : SelectTab<ItemRow, T> where T : class {
        private readonly ItemTabFilter Filter;

        public ItemTab( SelectDialog dialog, string name, string stateId, ItemTabFilter filter ) : base( dialog, name, stateId ) {
            Filter = filter;
        }

        protected override bool CheckMatch( ItemRow item, string searchInput ) => item.CheckMatch( searchInput );

        // ======== LOADING =========

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Item>() ) {
                if(
                    row.EquipSlotCategory.ValueNullable?.MainHand == 1 ||
                    row.EquipSlotCategory.ValueNullable?.OffHand == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Weapon ) ) continue;
                    var weapon = new ItemRowWeapon( row );
                    if( weapon.HasModel ) Items.Add( weapon );

                    if( !Filter.HasFlag( ItemTabFilter.SubWeapon ) ) continue;
                    if( weapon.HasSubModel ) Items.Add( weapon.SubItem );
                }
                else if(
                    row.EquipSlotCategory.ValueNullable?.Head == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Body == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Gloves == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Legs == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Feet == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Armor ) ) continue;
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
                else if(
                    row.EquipSlotCategory.ValueNullable?.Neck == 1 ||
                    row.EquipSlotCategory.ValueNullable?.FingerR == 1 ||
                    row.EquipSlotCategory.ValueNullable?.FingerL == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Wrists == 1 ||
                    row.EquipSlotCategory.ValueNullable?.Ears == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Accessory ) ) continue;
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }

            if( Filter.HasFlag( ItemTabFilter.Glasses ) ) {
                foreach( var row in Dalamud.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Glasses>() ) {
                    var glasses = new ItemRowArmor( row );
                    if( glasses.HasModel ) Items.Add( glasses );
                }
            }
        }
    }
}