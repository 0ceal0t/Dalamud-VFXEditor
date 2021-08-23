using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivCutscene {
        public string Name;
        public int RowId;
        public string Path;

        public XivCutscene( Lumina.Excel.GeneratedSheets.Cutscene cutscene ) {
            RowId = ( int )cutscene.RowId;
            var _path = cutscene.Path.ToString();
            var splitPath = _path.Split( '/' );
            Name = splitPath[0] + "/" + splitPath[^1]; // ffxiv/anvwil/anvwil00500/anvwil00500 -> ffxiv/anvwil00500
            Path = "cut/" + _path + ".cutb";
        }
    }
}
