using ImGuiNET;
using System.Linq;
using VfxEditor.Select.Shared;
using VfxEditor.Select.Shared.Cutscene;

namespace VfxEditor.Select.Vfx.Cutscene {
    public class CutsceneTab : SelectTab<CutsceneRow, ParseAvfx> {
        public CutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Cutscene", SelectResultType.GameCutscene ) { }

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) Items.Add( new CutsceneRow( item ) );
        }

        public override void LoadSelection( CutsceneRow item, out ParseAvfx loaded ) => ParseAvfx.ReadFile( item.Path, out loaded );

        protected override void DrawSelected() {
            ImGui.Text( "CUTB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.Path );

            DrawPaths( "VFX", Loaded.VfxPaths, Selected.Name, true );
        }

        protected override string GetName( CutsceneRow item ) => item.Name;
    }
}
