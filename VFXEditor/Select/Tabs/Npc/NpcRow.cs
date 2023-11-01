namespace VfxEditor.Select.Tabs.Npc {
    public enum NpcType {
        Demihuman = 2,
        Monster = 3
    }

    public class NpcRow {
        public readonly string Name;
        public readonly int RowId;

        public readonly int Key;
        public readonly int ModelId;
        public readonly int BaseId;
        public readonly int Variant;
        public readonly NpcType Type;

        public bool IsMonster => Type == NpcType.Monster;
        public string ModelString => ( IsMonster ? "m" : "d" ) + ModelId.ToString().PadLeft( 4, '0' );
        public string BaseIdString => BaseId.ToString().PadLeft( 4, '0' );
        public string PathPrefix => IsMonster ? "monster" : "demihuman";
        public string RootPath => "chara/" + PathPrefix + "/" + ModelString + ( IsMonster ? "/obj/body/b" : "/obj/equipment/e" ) + BaseIdString + "/";
        public string AtchPath => $"chara/xls/attachoffset/{ModelString}.atch";
        public string ImcPath => RootPath + ( IsMonster ? "b" : "e" ) + BaseIdString + ".imc";

        public NpcRow( Lumina.Excel.GeneratedSheets.ModelChara npc, string name ) {
            Name = name;
            RowId = ( int )npc.RowId;
            ModelId = npc.Model;
            BaseId = npc.Base;
            Variant = npc.Variant;
            Type = ( NpcType )npc.Type;
        }

        // chara/monster/m0624/obj/body/b0001/vfx/eff/vm0001.avfx
        // chara/demihuman/d1003/obj/equipment/e0006/vfx/eff/ve0006.avfx
        public string GetVfxPath( int vfxId ) => RootPath + "vfx/eff/v" + ( IsMonster ? "m" : "e" ) + vfxId.ToString().PadLeft( 4, '0' ) + ".avfx";

        public string GetSkeletonPath( string prefix, string extension ) => $"chara/{PathPrefix}/{ModelString}/skeleton/base/b0001/{prefix}_{ModelString}b0001.{extension}";
    }
}