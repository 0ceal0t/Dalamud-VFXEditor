using Lumina.Excel.GeneratedSheets2;

namespace VfxEditor.Select.Tabs.Items {
    public class ItemRowArmor : ItemRow {
        private readonly string ModelString;

        public override string ImcPath => $"chara/{Prefix}/{ModelString}/{ModelString}.imc";

        public override int Variant => Ids.GearVariant;

        public override string RootPath => $"chara/{Prefix}/{ModelString}/vfx/eff/v" + ( IsAccessory ? "a" : "e" );

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

        public bool IsAccessory =>
            Type == ItemType.Ears ||
            Type == ItemType.Neck ||
            Type == ItemType.RFinger ||
            Type == ItemType.LFinger ||
            Type == ItemType.Wrists;

        public string Prefix => IsAccessory ? "accessory" : "equipment";

        public ItemRowArmor( Item item ) : base( item ) {
            ModelString = ( IsAccessory ? "a" : "e" ) + $"{Ids.Id1:D4}";
        }

        public string GetMtrlPath( int id, string race, string suffix ) => $"chara/{Prefix}/{ModelString}/material/v{id:D4}/mt_{race}{ModelString}_{Suffix}_{suffix}.mtrl";

        public string GetMdlPath( string race ) => $"chara/{Prefix}/{ModelString}/model/{race}{ModelString}_{Suffix}.mdl";
    }
}