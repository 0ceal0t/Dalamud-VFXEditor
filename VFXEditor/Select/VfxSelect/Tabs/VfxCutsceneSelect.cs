using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxCutsceneSelect : SelectTab<XivCutscene, XivCutsceneSelected> {
        public VfxCutsceneSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Cutscenes, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "CUTB:" );
            ImGui.SameLine();
            DisplayPath( Loaded.Cutscene.Path );

            DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameCutscene, Loaded.Cutscene.Name, true );
        }

        protected override string GetName( XivCutscene item ) => item.Name;
    }
}