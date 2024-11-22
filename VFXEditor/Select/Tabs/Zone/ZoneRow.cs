using Lumina.Excel.Sheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Zone {
    public class ZoneRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly int PlaceRowId;
        public readonly string LgbPath;
        public readonly uint BgmId;

        public ZoneRow( TerritoryType zone ) {
            Name = zone.PlaceName.ValueNullable?.Name.ToString();
            RowId = ( int )zone.RowId;
            PlaceRowId = ( int )zone.PlaceName.ValueNullable?.RowId;
            BgmId = zone.BGM.RowId;

            var bg = zone.Bg.ToString().Split( '/' );
            bg[^1] = "vfx.lgb";
            LgbPath = "bg/" + string.Join( "/", bg );
        }

        public string GetName() => Name;
    }
}