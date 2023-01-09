using ImGuiNET;
using System.Diagnostics;
using VfxEditor.Utils;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxNpcSelect : SelectTab<XivNpc, XivNpcSelected> {
        public VfxNpcSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Npcs, dialog ) { }

        protected override bool CheckMatch( XivNpc item, string searchInput ) => Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );

        protected override void DrawExtra() => NpcThankYou();

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "Variant: " + Loaded.Npc.Variant );

            DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameNpc, Loaded.Npc.Name, true );
        }

        protected override string GetName( XivNpc item ) => item.Name;
    }
}