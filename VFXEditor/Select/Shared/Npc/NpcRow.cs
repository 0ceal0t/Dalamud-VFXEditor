namespace VfxEditor.Select.Shared.Npc {
    public enum NpcType {
        Demihuman = 2,
        Monster = 3
    }

    public class NpcRow {
        public string Name;
        public readonly int RowId;

        public readonly int Key;
        public readonly int ModelId;
        public readonly int BaseId;
        public readonly int Variant;
        public readonly NpcType Type;

        public readonly string ModelString;
        public readonly string BaseIdString;
        public readonly string RootPath;

        public string PathPrefix => Type == NpcType.Monster ? "monster" : "demihuman";

        public string AtchPath => $"chara/xls/attachoffset/{ModelString}.atch";

        public NpcRow( Lumina.Excel.GeneratedSheets.ModelChara npc ) : this( npc, null ) { }

        public NpcRow( Lumina.Excel.GeneratedSheets.ModelChara npc, string name ) {
            Name = name;
            RowId = ( int )npc.RowId;
            ModelId = npc.Model;
            BaseId = npc.Base;
            Variant = npc.Variant;
            Type = ( NpcType )npc.Type;
            BaseIdString = BaseId.ToString().PadLeft( 4, '0' );

            if( Type == NpcType.Monster ) {
                ModelString = "m" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/monster/" + ModelString + "/obj/body/b" + BaseIdString + "/";
            }
            else { // demihuman
                ModelString = "d" + ModelId.ToString().PadLeft( 4, '0' );
                RootPath = "chara/demihuman/" + ModelString + "/obj/equipment/e" + BaseIdString + "/";
            }
        }

        public string GetImcPath() => RootPath + ( Type == NpcType.Monster ? "b" : "e" ) + BaseIdString + ".imc";

        // chara/monster/m0624/obj/body/b0001/vfx/eff/vm0001.avfx
        // chara/demihuman/d1003/obj/equipment/e0006/vfx/eff/ve0006.avfx
        public string GetVfxPath( int vfxId ) => RootPath + "vfx/eff/v" + ( Type == NpcType.Monster ? "m" : "e" ) + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";

        public string GetSkeletonPath( string prefix, string extension ) => $"chara/{PathPrefix}/{ModelString}/skeleton/base/b0001/{prefix}_{ModelString}b0001.{extension}";
    }
}
