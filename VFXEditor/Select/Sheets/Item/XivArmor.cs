using System;
using System.Collections.Generic;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivArmor : XivItem {
        public XivArmor( Lumina.Excel.GeneratedSheets.Item item ) : base( item ) {
            RootPath = "chara/equipment/e" + Ids.Id1.ToString().PadLeft( 4, '0' ) + "/";
            VfxRootPath = RootPath + "vfx/eff/ve";
            ImcPath = RootPath + "e" + Ids.Id1.ToString().PadLeft( 4, '0' ) + ".imc";
            Variant = Ids.GearVariant;
        }
    }
}
