using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VfxEditor.Select.Scd.BgmQuest;

namespace VfxEditor.Select.Scd.Instance {
    public class InstanceRowSelected {
        public BgmSituationStruct Situation;
    }

    public class InstanceTab : SelectTab<InstanceRow, InstanceRowSelected> {
        public InstanceTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Instance", SelectResultType.GameMusic ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<ContentFinderCondition>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.Content > 0 && x.ContentLinkType == 1 );
            foreach( var item in sheet ) Items.Add( new InstanceRow( item ) );
        }

        public override void LoadSelection( InstanceRow item, out InstanceRowSelected loaded ) {
            var instance = Dalamud.DataManager.GetExcelSheet<InstanceContent>().GetRow( item.ContentRowId );
            loaded = new() {
                Situation = BgmQuestTab.GetBgmSituation( ( ushort )instance.BGM.Row )
            };
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Image );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawBgmSituation( Selected.Name, Loaded.Situation );
        }

        protected override string GetName( InstanceRow item ) => item.Name;
    }
}
