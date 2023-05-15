using System.Text;

namespace VfxEditor.Select.Shared.Item {
    public class WeaponRow : ItemRow {
        public bool HasSubModel;
        public WeaponRow SubItem = null;

        public string OverrideImcPath = null;
        private readonly string ModelString;
        private readonly string BodyString;

        public WeaponRow( Lumina.Excel.GeneratedSheets.Item item ) : base( item ) {
            HasSubModel = SecondaryIds.Id1 != 0;

            ModelString = "w" + Ids.Id.ToString().PadLeft( 4, '0' );
            BodyString = "b" + Ids.WeaponBody.ToString().PadLeft( 4, '0' );

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
                    // not sure why this requires it. sometimes the +50 model isn't in the submodel
                    ModelMain = doubleHand ? ItemIds.ToLong( Ids.Id1 + 50, Ids.Id2, Ids.Id3, Ids.Id4 ) : item.ModelSub,
                    ModelSub = 0
                };
                SubItem = new WeaponRow( subItem );

                if( doubleHand ) SubItem.OverrideImcPath = GetImcPath();
            }
        }

        public override string GetVfxRootPath() => $"chara/weapon/{ModelString}/obj/body/{BodyString}/vfx/eff/vw";

        public override string GetImcPath() => !string.IsNullOrEmpty( OverrideImcPath ) ? OverrideImcPath :
            $"chara/weapon/{ModelString}/obj/body/{BodyString}/{BodyString}.imc";

        public override int GetVariant() => Ids.WeaponVariant;

        public string GetPapPath() => $"chara/weapon/{ModelString}/animation/a0001/wp_common/resident/weapon.pap";
    }
}
