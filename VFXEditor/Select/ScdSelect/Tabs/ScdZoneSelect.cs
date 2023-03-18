using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdZoneSelect : SelectTab<XivZone, XivZoneScdSelected> {
        public ScdZoneSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.ZoneScd, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawBgmSituation( Loaded.Zone.Name, parentId, Loaded.Situation );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var quests = Loaded.Quests;
            var idx = 0;
            foreach( var entry in quests ) {
                var id = $"{parentId}{idx}";
                if( ImGui.CollapsingHeader( $"{entry.Key}{id}" ) ) {
                    ImGui.Indent();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                    DrawBgmSituation( entry.Key, id, entry.Value );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                    ImGui.Unindent();
                }
                idx++;
            }
        }

        protected override string GetName( XivZone item ) => item.Name;
    }
}