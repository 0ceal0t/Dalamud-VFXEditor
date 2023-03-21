using ImGuiNET;
using System;
using System.Linq;
using VfxEditor.Select2.Shared;

namespace VfxEditor.Select2.Vfx.Cutscene {
    public class CutsceneTab : SelectTab<CutsceneRow, ParseAvfxFromFile> {
        public CutsceneTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );

            foreach( var item in sheet ) Items.Add( new CutsceneRow( item ) );
        }

        public override void SelectItem( CutsceneRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.Path, out loaded );

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "CUTB:" );
            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Selected.Path );

            Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameCutscene, Selected.Name, true );
        }

        protected override string GetName( CutsceneRow item ) => item.Name;
    }
}
