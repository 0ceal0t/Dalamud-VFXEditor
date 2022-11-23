using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdOrchestrionSelect : SelectTab<XivOrchestrion, XivOrchestrionSelected> {
        public ScdOrchestrionSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.Orchestrions, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawPath( "Path", Loaded.Path, parentId, SelectResultType.GameMusic, Loaded.Orchestrion.Name );
        }

        protected override string GetName( XivOrchestrion item ) => item.Name;
    }
}