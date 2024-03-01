using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Cutscenes {
    public class CutsceneTab : SelectTab<CutsceneRow, ParsedPaths> {
        public CutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Cutscene", SelectResultType.GameCutscene ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) Items.Add( new CutsceneRow( item ) );
        }

        public override void LoadSelection( CutsceneRow item, out ParsedPaths loaded ) => ParsedPaths.ReadFile( item.Path, SelectDataUtils.AvfxRegex, out loaded );

        protected override void DrawSelected() {
            ImGui.Text( "CUTB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.Path );

            DrawPaths( Loaded.Paths, Selected.Name );
        }

        protected override string GetName( CutsceneRow item ) => item.Name;
    }
}