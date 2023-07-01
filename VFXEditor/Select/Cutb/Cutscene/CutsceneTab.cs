using System.Linq;
using VfxEditor.Select.Shared.Cutscene;

namespace VfxEditor.Select.Cutb.Cutscene {
    public class CutsceneTab : SelectTab<CutsceneRow> {
        public CutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Cutscene" ) { }

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );

            foreach( var item in sheet ) Items.Add( new CutsceneRow( item ) );
        }

        protected override void DrawSelected() {
            Dialog.DrawPath( "Path", Selected.Path, SelectResultType.GameCutscene, Selected.Name );
        }

        protected override string GetName( CutsceneRow item ) => item.Name;
    }
}
