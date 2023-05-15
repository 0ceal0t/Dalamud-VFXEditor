using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Scd.Instance {
    public class InstanceTab : SelectTab<InstanceRow, InstanceRowSelected> {
        public InstanceTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Instance" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<ContentFinderCondition>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.Content > 0 && x.ContentLinkType == 1 );

            foreach( var item in sheet ) Items.Add( new InstanceRow( item ) );
        }

        public override void LoadSelection( InstanceRow item, out InstanceRowSelected loaded ) {
            loaded = new( item );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Image );

        protected override void DrawSelected() {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawBgmSituation( Selected.Name, Loaded.Situation );
        }

        protected override string GetName( InstanceRow item ) => item.Name;
    }
}
