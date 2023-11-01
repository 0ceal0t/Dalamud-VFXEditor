using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.Items {
    public class ItemRowArmor : ItemRow {
        private readonly string ModelString;

        public override string ImcPath => $"chara/equipment/{ModelString}/{ModelString}.imc";

        public override int Variant => Ids.GearVariant;

        public override string RootPath => $"chara/equipment/{ModelString}/vfx/eff/ve";

        public string Suffix => Type switch {
            ItemType.Head => "met",
            ItemType.Hands => "glv",
            ItemType.Legs => "dwn",
            ItemType.Feet => "sho",
            ItemType.Body => "top",
            ItemType.Ears => "ear",
            ItemType.Neck => "nek",
            ItemType.RFinger => "rir",
            ItemType.LFinger => "ril",
            ItemType.Wrists => "wrs",
            _ => "unk",
        };

        public ItemRowArmor( Item item ) : base( item ) {
            ModelString = "e" + Ids.Id1.ToString().PadLeft( 4, '0' );
        }

        public string GetMtrlPath( string race, string suffix ) => $"chara/equipment/{ModelString}/material/{VariantString}/mt_{race}{ModelString}_{Suffix}_{suffix}.mtrl";
    }
}