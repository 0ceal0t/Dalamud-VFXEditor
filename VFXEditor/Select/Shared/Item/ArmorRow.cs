namespace VfxEditor.Select.Shared.Item {
    public class ArmorRow : ItemRow {
        private readonly string ModelString;

        public ArmorRow( Lumina.Excel.GeneratedSheets.Item item ) : base( item ) {
            ModelString = "e" + Ids.Id1.ToString().PadLeft( 4, '0' );
        }

        public override string GetImcPath() => $"chara/equipment/{ModelString}/{ModelString}.imc";

        public override int GetVariant() => Ids.GearVariant;

        public override string GetVfxRootPath() => $"chara/equipment/{ModelString}/vfx/eff/ve";
    }
}
