using Lumina.Excel.Sheets;
using System;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.NpcID {
    public enum NpcType {
        Demihuman = 2,
        Monster = 3
    }

    public class NpcRow : ISelectItem {
        public readonly string Name;
        public readonly int ModelId;
        public readonly NpcType Type;

        public bool IsMonster => Type == NpcType.Monster;
        public string ModelString => ( IsMonster ? "m" : "d" ) + $"{ModelId:D4}";

        public NpcRow( string path ) {
            Name = path;
            ModelId = Convert.ToInt32(path.Substring(1,4));
            Type = NpcType.Monster;
            if( path.Substring( 0, 1 ) == "d") { Type = NpcType.Demihuman; }
        }
        public string GetName() => Name;
    }
}