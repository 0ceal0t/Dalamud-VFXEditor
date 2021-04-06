using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor {
    public class XivCutscene {
        public string Name;
        public int RowId;
        public string Path;
        
        public XivCutscene( Lumina.Excel.GeneratedSheets.Cutscene cutscene ) {
            RowId = ( int )cutscene.RowId;
            Name = cutscene.Path.ToString();
            Path = "cut/" + Name + ".cutb";
        }
    }
}
