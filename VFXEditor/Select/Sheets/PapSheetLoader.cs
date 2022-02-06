using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class PapSheetLoader : SheetLoader<XivPap, XivPapSelected> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivPap( item ) );
            }
        }

        public override bool SelectItem( XivPap item, out XivPapSelected selectedItem ) {
            selectedItem = new XivPapSelected(
                item,
                FilterByExists( XivPap.GenerateAllSkeletons( item.StartPap ) ),
                FilterByExists( XivPap.GenerateAllSkeletons( item.EndPap ) ),
                FilterByExists( XivPap.GenerateAllSkeletons( item.HitPap ) )
            );
            return true;
        }

        private Dictionary<string, string> FilterByExists( Dictionary<string, string> dict ) {
            return dict.Where( i => SheetManager.DataManager.FileExists( i.Value ) ).ToDictionary( i => i.Key, i => i.Value );
        }
    }
}
