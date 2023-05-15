using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Select.Shared.Common;

namespace VfxEditor.Select.Vfx.Common {
    public class CommonTab : SelectTab<CommonRow> {
        public CommonTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Common" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            Items.AddRange( new List<CommonRow>() {
                new( 0, "vfx/action/ab_swd_abl020/eff/abi_swd020c1t.avfx", "Passage of Arms", 2515 ),
                new( 1, "vfx/action/ab_2gn026/eff/ab_2gn026c0c.avfx", "Flamethrower", 3038 ),
                new( 2, "vfx/action/ab_2kt008/eff/ab_2kt008c1t.avfx", "Meditate", 3172 ),
                new( 3, "vfx/aoz/mgc_rod084/eff/mgc_rod084c0d1.avfx", "Chelonian Gate", 3340 ),
                new( 4, "vfx/common/eff/2sw_dd_field01s.avfx", "Salted Earth", 3066 ),
                new( 5, "vfx/common/eff/mgc_dg05d1d0t.avfx", "Doton", 2911 ),
                new( 6, "vfx/common/eff/ab_dgr025c1t.avfx", "Ten Chi Jin", 2922 ),
                new( 7, "vfx/action/ab_clw_abl026/eff/ab_clw026c0t.avfx", "Anatman", 2546 ),
                new( 8, "vfx/monster/m7103/eff/m7103sp_01c0h.avfx", "Stellar Explosion", 405 ),
                new( 9, "chara/monster/m7103/obj/body/b0001/vfx/eff/vm0001.avfx", "Earthly Star", 3143 ),
                new( 10, "vfx/common/eff/abi_bari1c0h.avfx", "Collective Unconscious", 3140 ),
                new( 11, "vfx/common/eff/abi_gt_heal0f.avfx", "Asylum", 2632 ),
                new( 12, "vfx/common/eff/abi_yase1d0c.avfx", "Sacred Soil", 2804 ),
                new( 13, "vfx/common/eff/m7004sp_05d0t.avfx", "Earthen Fury", 2705 ),
                new( 14, "vfx/common/eff/m7005sp_32d0t.avfx", "Slipstream", 2716 ),
                new( 15, "vfx/common/eff/ab_chk012c0c.avfx", "Improvisation", 3477 ),
            } );

            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.VFX>().Where( x => !string.IsNullOrEmpty( x.Location ) );
            foreach( var item in sheet ) Items.Add( new( ( int )item.RowId, $"vfx/common/eff/{item.Location}.avfx", item.Location.ToString(), 0 ) );

            var lineIdx = 0;
            foreach( var line in File.ReadLines( SelectUtils.MiscVfxPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                Items.Add( new CommonRow( lineIdx, line, line.Replace( ".avfx", "" ), 0 ) );
                lineIdx++;
            }
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPath( "Path", Selected.Path, SelectResultType.GameMisc, Selected.Name, true );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}
