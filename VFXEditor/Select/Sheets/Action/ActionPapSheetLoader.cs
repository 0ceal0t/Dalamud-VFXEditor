using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class ActionPapSheetLoader : SheetLoader<XivActionPap, XivActionPapSelected> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivActionPap( item ) );
            }
        }

        public override bool SelectItem( XivActionPap item, out XivActionPapSelected selectedItem ) {
            selectedItem = new XivActionPapSelected(
                item,
                FilterByExists( XivActionPap.GenerateAllSkeletons( item.StartPap ) ),
                FilterByExists( XivActionPap.GenerateAllSkeletons( item.EndPap ) ),
                FilterByExists( XivActionPap.GenerateAllSkeletons( item.HitPap ) )
            );
            return true;
        }

        public static Dictionary<string, string> FilterByExists( Dictionary<string, string> dict ) {
            return dict.Where( i => SheetManager.DataManager.FileExists( i.Value ) ).ToDictionary( i => i.Key, i => i.Value );
        }
    }
}
