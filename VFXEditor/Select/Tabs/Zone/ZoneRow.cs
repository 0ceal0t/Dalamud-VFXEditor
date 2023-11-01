using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.Zone {
    public class ZoneRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly int PlaceRowId;
        public readonly string LgbPath;
        public readonly ushort BgmId;

        public ZoneRow( TerritoryType zone ) {
            Name = zone.PlaceName.Value?.Name.ToString();
            RowId = ( int )zone.RowId;
            PlaceRowId = ( int )zone.PlaceName.Value?.RowId;
            BgmId = zone.BGM;

            var bg = zone.Bg.ToString().Split( '/' );
            bg[^1] = "vfx.lgb";
            LgbPath = "bg/" + string.Join( "/", bg );
        }
    }
}