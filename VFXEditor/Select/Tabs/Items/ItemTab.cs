using System;

namespace VfxEditor.Select.Tabs.Items {
    [Flags]
    public enum ItemTabFilter {
        Weapon = 0x01,
        SubWeapon = 0x02,
        Armor = 0x04,
        Acc = 0x08,
    }

    public abstract class ItemTab<T> : SelectTab<ItemRow, T> where T : class {
        private readonly ItemTabFilter Filter;

        public ItemTab( SelectDialog dialog, string name, string stateId, ItemTabFilter filter ) : base( dialog, name, stateId ) {
            Filter = filter;
        }

        // ======== LOADING =========

        public override void LoadData() {
            foreach( var row in Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>() ) {
                if(
                    row.EquipSlotCategory.Value?.MainHand == 1 ||
                    row.EquipSlotCategory.Value?.OffHand == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Weapon ) ) continue;
                    var weapon = new ItemRowWeapon( row );
                    if( weapon.HasModel ) Items.Add( weapon );

                    if( !Filter.HasFlag( ItemTabFilter.SubWeapon ) ) continue;
                    if( weapon.HasSubModel ) Items.Add( weapon.SubItem );
                }
                else if(
                    row.EquipSlotCategory.Value?.Head == 1 ||
                    row.EquipSlotCategory.Value?.Body == 1 ||
                    row.EquipSlotCategory.Value?.Gloves == 1 ||
                    row.EquipSlotCategory.Value?.Legs == 1 ||
                    row.EquipSlotCategory.Value?.Feet == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Armor ) ) continue;
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
                else if(
                    row.EquipSlotCategory.Value?.Neck == 1 ||
                    row.EquipSlotCategory.Value?.FingerR == 1 ||
                    row.EquipSlotCategory.Value?.FingerL == 1 ||
                    row.EquipSlotCategory.Value?.Wrists == 1 ||
                    row.EquipSlotCategory.Value?.Ears == 1
                ) {
                    if( !Filter.HasFlag( ItemTabFilter.Acc ) ) continue;
                    var armor = new ItemRowArmor( row );
                    if( armor.HasModel ) Items.Add( armor );
                }
            }
        }

        protected override string GetName( ItemRow item ) => item.Name;

        protected override uint GetIconId( ItemRow item ) => item.Icon;
    }
}