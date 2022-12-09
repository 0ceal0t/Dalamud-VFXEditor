using System;
using System.Collections.Generic;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivWeapon : XivItem {
        public bool HasSubModel;
        public XivWeapon SubItem = null;

        public XivWeapon( Lumina.Excel.GeneratedSheets.Item item ) : base(item) {
            HasSubModel = ( SecondaryIds.Id1 != 0 );

            RootPath = $"chara/weapon/w" + Ids.Id.ToString().PadLeft( 4, '0' ) + "/obj/body/b" + Ids.WeaponBody.ToString().PadLeft( 4, '0' ) + "/";
            VfxRootPath = RootPath + "vfx/eff/vw";
            ImcPath = RootPath + "b" + Ids.WeaponBody.ToString().PadLeft( 4, '0' ) + ".imc";
            Variant = Ids.WeaponVariant;

            if( HasSubModel ) {
                var category = item.ItemUICategory.Value.RowId;
                var doubleHand = ( category == 1 || category == 84 || category == 107 ); // MNK, NIN, DNC weapons

                var subItem = new Lumina.Excel.GeneratedSheets.Item {
                    Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Offhand" ) ),
                    Icon = item.Icon,
                    EquipRestriction = item.EquipRestriction,
                    EquipSlotCategory = item.EquipSlotCategory,
                    ItemSearchCategory = item.ItemSearchCategory,
                    ItemSortCategory = item.ItemSortCategory,
                    ClassJobCategory = item.ClassJobCategory,
                    ItemUICategory = item.ItemUICategory,
                    ModelMain = doubleHand ? XivItemIds.ToItemsId( Ids.Id1 + 50, Ids.Id2, Ids.Id3, Ids.Id4 ) : item.ModelSub, // not sure why this requires it. sometimes the +50 model isn't in the submodel
                    ModelSub = 0
                };
                SubItem = new XivWeapon( subItem );

                if( doubleHand ) SubItem.ImcPath = ImcPath;
            }
        }
    }
}
