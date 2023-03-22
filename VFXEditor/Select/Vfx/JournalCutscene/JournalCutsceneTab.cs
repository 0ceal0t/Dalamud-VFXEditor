using ImGuiNET;
using System;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.JournalCutscene {
    public class JournalCutsceneTab : SelectTab<JournalCutsceneRow, ParseAvfxFromFile> {
        public JournalCutsceneTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-JournalCutscene" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.CompleteJournal>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new JournalCutsceneRow( item ) );
        }

        public override void LoadSelection( JournalCutsceneRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.Paths, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            for( var idx = 0; idx < Loaded.VfxPaths.Count; idx++ ) {
                var paths = Loaded.VfxPaths[idx];
                if( ImGui.CollapsingHeader( $"Cutscene {idx}{parentId}" ) ) {
                    ImGui.Indent();
                    Dialog.DrawPath( "VFX", paths, $"{parentId}{idx}", SelectResultType.GameCutscene, $"{Selected.Name} {idx}", true );
                    ImGui.Unindent();
                }
            }
        }

        protected override string GetName( JournalCutsceneRow item ) => item.Name;
    }
}
