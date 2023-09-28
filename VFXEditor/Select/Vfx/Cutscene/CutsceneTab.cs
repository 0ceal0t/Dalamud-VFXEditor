using ImGuiNET;
using System.Linq;
using VfxEditor.Select.Shared;
using VfxEditor.Select.Shared.Cutscene;

namespace VfxEditor.Select.Vfx.Cutscene {
    public class CutsceneTab : SelectTab<CutsceneRow, ParsedPaths> {
        public CutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Cutscene", SelectResultType.GameCutscene ) { }

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) Items.Add( new CutsceneRow( item ) );
        }

        public override void LoadSelection( CutsceneRow item, out ParsedPaths loaded ) => ParsedPaths.ReadFile( item.Path, SelectDataUtils.AvfxRegex, out loaded );

        protected override void DrawSelected() {
            ImGui.Text( "CUTB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.Path );

            DrawPaths( "VFX", Loaded.Paths, Selected.Name, true );
        }

        protected override string GetName( CutsceneRow item ) => item.Name;
    }
}
