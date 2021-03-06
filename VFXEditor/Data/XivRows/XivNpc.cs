using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor {

    public class NpcCsvRow {
        public int Id;
        public string Name;
    }

    public enum NpcType {
        //None = 0,
        //Player = 1,
        Demihuman = 2,
        Monster = 3,
        //Object = 4,
        //Unknown = 5
    }

    public class XivNpc {
        public string Name;
        public bool CSV_Defined = false;
        public int RowId;

        public int Key;
        public int ModelId;
        public int BaseId;
        public int Variant;
        public NpcType Type;

        public string RootPath;

        public XivNpc( Lumina.Excel.GeneratedSheets.ModelChara npc, Dictionary<int, string> NpcIdToName ) {
            RowId = ( int )npc.RowId;
            ModelId = npc.Model;
            BaseId = npc.Base;
            Variant = npc.Variant;
            Type = ( NpcType )npc.Type;

            CSV_Defined = NpcIdToName.ContainsKey( RowId );
            if( CSV_Defined ) {
                Name = NpcIdToName[RowId];
            }
            else {
                return;
            }

            // chara/monster/m0230/obj/body/b0001/b0001.imc
            // chara/demihuman/d0001/obj/equipment/e0000/e0000.imc
            // chara/demihuman/d0001/obj/equipment/e0001/e0001.imc

            if(Type == NpcType.Monster ) {
                RootPath = "chara/monster/m" + ModelId.ToString().PadLeft( 4, '0' ) + "/obj/body/b" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
            }
            else { // demihuman
                RootPath = "chara/demihuman/d" + ModelId.ToString().PadLeft( 4, '0' ) + "/obj/equipment/e" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
            }
        }

        public string GetImcPath() {
            if( Type == NpcType.Monster ) {
                return RootPath + "b" + BaseId.ToString().PadLeft( 4, '0' ) + ".imc";
            }
            else { // demihuman
                return RootPath + "e" + BaseId.ToString().PadLeft( 4, '0' ) + ".imc";
            }
        }

        public string GetTmbPath() {
            // chara/action/mon_sp/d1028
            string prefix = ( Type == NpcType.Monster ) ? "m" : "d";
            return "chara/action/mon_sp/" + prefix + ModelId.ToString().PadLeft( 4, '0' ) + "/";
        }

        public string GetVfxPath(int vfxId ) {
            if( Type == NpcType.Monster ) { // chara/monster/m0624/obj/body/b0001/vfx/eff/vm0001.avfx
                return RootPath + "vfx/eff/vm" + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";
            }
            else { // demihuman chara/demihuman/d1003/obj/equipment/e0006/vfx/eff/ve0006.avfx
                return RootPath + "vfx/eff/ve" + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";
            }
        }
    }
}
