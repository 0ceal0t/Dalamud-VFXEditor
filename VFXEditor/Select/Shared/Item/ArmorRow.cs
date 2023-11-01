namespace VfxEditor.Select.Shared.Item {
    public class ArmorRow : ItemRow {
        private readonly string ModelString;

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

        public ArmorRow( Lumina.Excel.GeneratedSheets.Item item ) : base( item ) {
            ModelString = "e" + Ids.Id1.ToString().PadLeft( 4, '0' );
        }

        public override string GetImcPath() => $"chara/equipment/{ModelString}/{ModelString}.imc";

        public override int GetVariant() => Ids.GearVariant;

        public override string GetVfxRootPath() => $"chara/equipment/{ModelString}/vfx/eff/ve";

        public string GetMtrlPath( string race, string suffix ) => $"chara/equipment/{ModelString}/material/{VariantString}/mt_{race}{ModelString}_{Suffix}_{suffix}.mtrl";
    }
}
