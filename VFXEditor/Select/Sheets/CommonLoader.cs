using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class CommonLoader : SheetLoader<XivCommon, XivCommon> {
        public override void OnLoad() {
            Items = new() {
                new XivCommon( 0, "vfx/action/ab_swd_abl020/eff/abi_swd020c1t.avfx", "Passage of Arms", 2515 ),
                new XivCommon( 1, "vfx/action/ab_2gn026/eff/ab_2gn026c0c.avfx", "Flamethrower", 3038 ),
                new XivCommon( 2, "vfx/action/ab_2kt008/eff/ab_2kt008c1t.avfx", "Meditate", 3172 ),
                new XivCommon( 3, "vfx/aoz/mgc_rod084/eff/mgc_rod084c0d1.avfx", "Chelonian Gate", 3340 ),
                new XivCommon( 4, "vfx/common/eff/2sw_dd_field01s.avfx", "Salted Earth", 3066 ),
                new XivCommon( 5, "vfx/common/eff/mgc_dg05d1d0t.avfx", "Doton", 2911 ),
                new XivCommon( 6, "vfx/common/eff/ab_dgr025c1t.avfx", "Ten Chi Jin", 2922 ),
                new XivCommon( 7, "vfx/action/ab_clw_abl026/eff/ab_clw026c0t.avfx", "Anatman", 2546 ),
                new XivCommon( 8, "vfx/monster/m7103/eff/m7103sp_01c0h.avfx", "Stellar Explosion", 405 ),
                new XivCommon( 9, "chara/monster/m7103/obj/body/b0001/vfx/eff/vm0001.avfx", "Earthly Star", 3143 ),
                new XivCommon( 10, "vfx/common/eff/abi_bari1c0h.avfx", "Collective Unconscious", 3140 ),
                new XivCommon( 11, "vfx/common/eff/abi_gt_heal0f.avfx", "Asylum", 2632 ),
                new XivCommon( 12, "vfx/common/eff/abi_yase1d0c.avfx", "Sacred Soil", 2804 ),
                new XivCommon( 13, "vfx/common/eff/m7004sp_05d0t.avfx", "Earthen Fury", 2705 ),
                new XivCommon( 14, "vfx/common/eff/m7005sp_32d0t.avfx", "Slipstream", 2716 ),
                new XivCommon( 15, "vfx/common/eff/ab_chk012c0c.avfx", "Improvisation", 3477 ),
            };

            var sheet = SheetManager.DataManager.GetExcelSheet<VFX>().Where( x => !string.IsNullOrEmpty(x.Location) );
            foreach( var item in sheet ) {
                Items.Add( new XivCommon(item) );
            }
        }

        public override bool SelectItem( XivCommon item, out XivCommon selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}