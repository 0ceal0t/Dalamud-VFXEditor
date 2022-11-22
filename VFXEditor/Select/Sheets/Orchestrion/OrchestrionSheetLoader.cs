using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class OrchestrionSheeetLoader : SheetLoader<XivOrchestrion, XivOrchestrionSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Orchestrion>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new XivOrchestrion( item ) );
        }

        public override bool SelectItem( XivOrchestrion item, out XivOrchestrionSelected selectedItem ) {
            selectedItem = new( item );
            return true;
        }
    }
}
