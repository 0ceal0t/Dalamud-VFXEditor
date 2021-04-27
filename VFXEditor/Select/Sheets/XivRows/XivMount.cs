using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivMount : XivNpcBase {
        public ushort Icon;

        public XivMount( Lumina.Excel.GeneratedSheets.Mount mount ) : base(mount.ModelChara.Value) {
            Icon = mount.Icon;
            Name = mount.Singular.ToString();
        }
    }
}
