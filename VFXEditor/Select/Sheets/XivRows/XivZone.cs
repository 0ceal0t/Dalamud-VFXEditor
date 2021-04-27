using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {

    public class XivZone {
        public string Name;
        public int RowId;

        public string LgbPath;

        // ffxiv/fst_f1/bah/f1bz/level/f1bz
        // bg/ffxiv/fst_f1/bah/f1bz/level/vfx.lgb

        public XivZone( Lumina.Excel.GeneratedSheets.TerritoryType zone ) {
            Name = zone.PlaceName.Value?.Name.ToString();
            RowId = ( int )zone.RowId;

            var bg = zone.Bg.ToString().Split( '/' );
            bg[bg.Length - 1] = "vfx.lgb";
            LgbPath = "bg/" + string.Join( "/", bg );
        }

        public string GetLgbPath() {
            return LgbPath;
        }
    }
}
