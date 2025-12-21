using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.JournalCutscene {
    public class JournalCutsceneTab : SelectTab<JournalCutsceneRow, List<ParsedPaths>> {
        public JournalCutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "JournalCutscene" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<CompleteJournal>().Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) );
            foreach( var item in sheet ) Items.Add( new JournalCutsceneRow( item ) );
        }

        public override void LoadSelection( JournalCutsceneRow item, out List<ParsedPaths> loaded ) => ParsedPaths.ReadFile( item.Paths, SelectDataUtils.AvfxRegex, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            for( var idx = 0; idx < Loaded.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );

                if( ImGui.CollapsingHeader( $"Cutscene {idx}" ) ) {
                    using var indent = ImRaii.PushIndent( 10f );
                    Dialog.DrawPaths( Loaded[idx].Paths, $"{Selected.Name} {idx}", SelectResultType.GameCutscene );
                }
            }
        }
    }
}