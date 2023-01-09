using ImGuiNET;
using System.Diagnostics;
using VfxEditor.Utils;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapNpcSelect : SelectTab<XivNpc, XivNpcSelected> {
        public PapNpcSelect( string tabId, PapSelectDialog dialog ) : base( tabId, SheetManager.Npcs, dialog ) { }

        protected override bool CheckMatch( XivNpc item, string searchInput ) => Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );

        protected override void DrawExtra() => NpcThankYou();

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Loaded.Npc.Variant );
            DrawPath( "PAP", Loaded.PapPaths, parentId, SelectResultType.GameNpc, Loaded.Npc.Name );
        }

        protected override string GetName( XivNpc item ) => item.Name;
    }
}