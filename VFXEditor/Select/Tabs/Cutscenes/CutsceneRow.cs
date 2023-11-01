using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.Cutscenes {
    public class CutsceneRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly string Path;

        public CutsceneRow( Cutscene cutscene ) {
            RowId = ( int )cutscene.RowId;
            var path = cutscene.Path.ToString();
            var splitPath = path.Split( '/' );
            Name = $"{splitPath[0]}/{splitPath[^1]}";
            // ffxiv/anvwil/anvwil00500/anvwil00500 -> ffxiv/anvwil00500
            Path = $"cut/{path}.cutb";
        }
    }
}