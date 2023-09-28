using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared;
using VfxEditor.Select.Shared.Cutscene;

namespace VfxEditor.Select.Vfx.JournalCutscene {
    public class JournalCutsceneTab : SelectTab<JournalCutsceneRow, List<ParsedPaths>> {
        public JournalCutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-JournalCutscene", SelectResultType.GameCutscene ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.CompleteJournal>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new JournalCutsceneRow( item ) );
        }

        public override void LoadSelection( JournalCutsceneRow item, out List<ParsedPaths> loaded ) => ParsedPaths.ReadFile( item.Paths, SelectDataUtils.AvfxRegex, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            for( var idx = 0; idx < Loaded.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );

                if( ImGui.CollapsingHeader( $"Cutscene {idx}" ) ) {
                    using var indent = ImRaii.PushIndent( 10f );
                    DrawPaths( "VFX", Loaded[idx].Paths, $"{Selected.Name} {idx}", true );
                }
            }
        }

        protected override string GetName( JournalCutsceneRow item ) => item.Name;
    }
}
