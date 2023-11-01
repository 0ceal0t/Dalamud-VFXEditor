using Lumina.Excel.GeneratedSheets;
using System.Text;

namespace VfxEditor.Select.Tabs.Items {
    public class ItemRowWeapon : ItemRow {
        public readonly bool HasSubModel;
        public readonly ItemRowWeapon SubItem = null;
        public readonly string OverrideImcPath = null;
        private readonly string ModelString;
        private readonly string BodyString;

        public override string RootPath => $"chara/weapon/{ModelString}/obj/body/{BodyString}/vfx/eff/vw";

        public override string ImcPath => !string.IsNullOrEmpty( OverrideImcPath ) ? OverrideImcPath : $"chara/weapon/{ModelString}/obj/body/{BodyString}/{BodyString}.imc";

        public override int Variant => Ids.WeaponVariant;

        public string PapPath => $"chara/weapon/{ModelString}/animation/a0001/wp_common/resident/weapon.pap";

        public ItemRowWeapon( Item item, string imcPath = "" ) : base( item ) {
            OverrideImcPath = imcPath;
            HasSubModel = SecondaryIds.Id1 != 0;
            ModelString = "w" + Ids.Id.ToString().PadLeft( 4, '0' );
            BodyString = "b" + Ids.WeaponBody.ToString().PadLeft( 4, '0' );

            // ======================================

            if( !HasSubModel ) return;
            var category = item.ItemUICategory.Value.RowId;
            var doubleHand = ( category == 1 || category == 84 || category == 107 ); // MNK, NIN, DNC weapons

            var subItem = new Item {
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
            SubItem = new ItemRowWeapon( subItem, doubleHand ? ImcPath : null );
        }

        public string GetMtrlPath( string suffix ) => $"chara/weapon/{ModelString}/obj/body/{BodyString}/material/{VariantString}/mt_{ModelString}{BodyString}_{suffix}.mtrl";
    }
}