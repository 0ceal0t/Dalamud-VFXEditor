namespace VFXEditor.Select.Rows {
    public enum NpcType {
        Demihuman = 2,
        Monster = 3
    }

    public class XivNpc {
        public string Name;
        public int RowId;

        public int Key;
        public int ModelId;
        public int BaseId;
        public int Variant;
        public NpcType Type;

        public string RootPath;
        public string Id;

        public XivNpc( Lumina.Excel.GeneratedSheets.ModelChara npc ) : this( npc, null) { }

        public XivNpc( Lumina.Excel.GeneratedSheets.ModelChara npc, string name ) {
            Name = name;
            RowId = ( int )npc.RowId;
            ModelId = npc.Model;
            BaseId = npc.Base;
            Variant = npc.Variant;
            Type = ( NpcType )npc.Type;

            if( Type == NpcType.Monster ) {
                Id = "m" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/monster/" + Id + "/obj/body/b" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
            }
            else { // demihuman
                Id = "d" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/demihuman/" + Id + "/obj/equipment/e" + BaseId.ToString().PadLeft( 4, '0' ) + "/";
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
            var prefix = ( Type == NpcType.Monster ) ? "m" : "d";
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
