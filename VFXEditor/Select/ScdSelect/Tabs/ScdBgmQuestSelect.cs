using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmQuestSelect : SelectTab<XivBgmQuest, XivBgmQuestSelected> {
        public ScdBgmQuestSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.BgmQuest, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawBgmSituation( Loaded.BgmQuest.Name, parentId, Loaded.Situation );
        }

        protected override string GetName( XivBgmQuest item ) => item.Name;
    }
}