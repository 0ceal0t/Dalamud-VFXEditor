using System.Linq;
using VfxEditor.Select.Shared.Cutscene;

namespace VfxEditor.Select.Cutb.JournalCutscene {
    public class JournalCutsceneTab : SelectTab<JournalCutsceneRow> {
        public JournalCutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-JournalCutscene" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.CompleteJournal>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new JournalCutsceneRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( "Path", Selected.Paths, SelectResultType.GameCutscene, Selected.Name );
        }

        protected override string GetName( JournalCutsceneRow item ) => item.Name;
    }
}
