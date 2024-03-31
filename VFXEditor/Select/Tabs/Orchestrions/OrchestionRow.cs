using Lumina.Excel.GeneratedSheets2;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Orchestrions {
    public class OrchestrionRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;

        public OrchestrionRow( Orchestrion orchestrion ) {
            Name = orchestrion.Name.ToString();
            RowId = ( int )orchestrion.RowId;
        }

        public string GetName() => Name;
    }
}