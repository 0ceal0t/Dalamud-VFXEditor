using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;

namespace VfxEditor.Select.Scd.BgmQuest {
    public class BgmQuestTab : SelectTab<BgmQuestRow, BgmQuestRowSelected> {
        public BgmQuestTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-BgmQuest" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.Quest.Row > 0 );

            foreach( var item in sheet ) Items.Add( new BgmQuestRow( item ) );
        }

        public override void LoadSelection( BgmQuestRow item, out BgmQuestRowSelected loaded ) {
            loaded = new( item );
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawBgmSituation( Selected.Name, parentId, Loaded.Situation );
        }

        protected override string GetName( BgmQuestRow item ) => item.Name;
    }
}
