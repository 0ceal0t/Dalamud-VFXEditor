namespace VfxEditor.Select.Shared.Npc {
    public class NpcRow {
        public enum NpcType {
            Demihuman = 2,
            Monster = 3
        }

        public string Name;
        public readonly int RowId;

        public readonly int Key;
        public readonly int ModelId;
        public readonly int BaseId;
        public readonly int Variant;
        public readonly NpcType Type;

        public readonly string MonsterId;
        public readonly string RootPath;

        public NpcRow( Lumina.Excel.GeneratedSheets.ModelChara npc ) : this( npc, null ) { }

        public NpcRow( Lumina.Excel.GeneratedSheets.ModelChara npc, string name ) {
            Name = name;
            RowId = ( int )npc.RowId;
            ModelId = npc.Model;
            BaseId = npc.Base;
            Variant = npc.Variant;
            Type = ( NpcType )npc.Type;

            if( Type == NpcType.Monster ) {
                MonsterId = "m" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/monster/" + MonsterId + "/obj/body/b" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
            }
            else { // demihuman
                MonsterId = "d" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/demihuman/" + MonsterId + "/obj/equipment/e" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
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
            var prefix = Type == NpcType.Monster ? "m" : "d";
            return "chara/action/mon_sp/" + prefix + ModelId.ToString().PadLeft( 4, '0' ) + "/";
        }

        public string GetVfxPath( int vfxId ) {
            if( Type == NpcType.Monster ) { // chara/monster/m0624/obj/body/b0001/vfx/eff/vm0001.avfx
                return RootPath + "vfx/eff/vm" + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";
            }
            else { // demihuman chara/demihuman/d1003/obj/equipment/e0006/vfx/eff/ve0006.avfx
                return RootPath + "vfx/eff/ve" + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";
            }
        }
    }
}
