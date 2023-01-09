using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbNpcSelect : SelectTab<XivNpc, XivNpcSelected> {
        public TmbNpcSelect( string tabId, TmbSelectDialog dialog ) : base( tabId, SheetManager.Npcs, dialog ) { }

        protected override bool CheckMatch( XivNpc item, string searchInput ) => Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );

        protected override void DrawExtra() => NpcThankYou();

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Loaded.Npc.Variant );
            DrawPath( "TMB", Loaded.TmbPaths, parentId, SelectResultType.GameNpc, Loaded.Npc.Name, true );
        }

        protected override string GetName( XivNpc item ) => item.Name;
    }
}