using Lumina.Excel.GeneratedSheets2;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Housing {
    public class HousingRow : ISelectItemWithIcon {
        public readonly string Name;
        public readonly string SgbPath;
        public readonly int RowId;
        public readonly ushort Icon;

        // bgcommon/hou/indoor/general/0694/asset/fun_b0_m0694.sgb
        // bgcommon/hou/outdoor/general/0114/asset/gar_b0_m0114.sgb

        public HousingRow( HousingYardObject item ) {
            Name = item.Item.Value.Name.ToString();
            RowId = ( int )item.Item.Value.RowId;
            Icon = item.Item.Value.Icon;

            var model = item.ModelKey;
            SgbPath = $"bgcommon/hou/outdoor/general/{model.ToString().PadLeft( 4, '0' )}/asset/gar_b0_m{model.ToString().PadLeft( 4, '0' )}.sgb";
        }

        public HousingRow( HousingFurniture item ) {
            Name = item.Item.Value.Name.ToString();
            RowId = ( int )item.Item.Value.RowId;
            Icon = item.Item.Value.Icon;

            var model = item.ModelKey;
            SgbPath = $"bgcommon/hou/indoor/general/{model.ToString().PadLeft( 4, '0' )}/asset/fun_b0_m{model.ToString().PadLeft( 4, '0' )}.sgb";
        }

        public string GetName() => Name;
        public uint GetIconId() => Icon;
    }
}