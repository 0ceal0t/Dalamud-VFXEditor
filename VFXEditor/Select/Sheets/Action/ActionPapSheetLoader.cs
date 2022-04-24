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
                SheetManager.FilterByExists( CalculateAllSkeletonPaths( item.StartPap ) ),
                SheetManager.FilterByExists( CalculateAllSkeletonPaths( item.EndPap ) ),
                SheetManager.FilterByExists( CalculateAllSkeletonPaths( item.HitPap ) )
            );
            return true;
        }

        public static string CalculateSkeletonPath( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> CalculateAllSkeletonPaths( string path ) {
            Dictionary<string, string> ret = new();
            if( string.IsNullOrEmpty( path ) ) return ret;

            ret.Add( "Midlander M", CalculateSkeletonPath( "c0101", path ) );
            ret.Add( "Midlander F", CalculateSkeletonPath( "c0201", path ) );
            ret.Add( "Highlander M", CalculateSkeletonPath( "c0301", path ) );
            ret.Add( "Highlander F", CalculateSkeletonPath( "c0401", path ) );
            ret.Add( "Elezen M", CalculateSkeletonPath( "c0501", path ) );
            ret.Add( "Elezen F", CalculateSkeletonPath( "c0601", path ) );
            ret.Add( "Miquote M", CalculateSkeletonPath( "c0701", path ) );
            ret.Add( "Miquote F", CalculateSkeletonPath( "c0801", path ) );
            ret.Add( "Roegadyn M", CalculateSkeletonPath( "c0901", path ) );
            ret.Add( "Roegadyn F", CalculateSkeletonPath( "c1001", path ) );
            ret.Add( "Lalafell M", CalculateSkeletonPath( "c1101", path ) );
            ret.Add( "Lalafell F", CalculateSkeletonPath( "c1201", path ) );
            ret.Add( "AuRa M", CalculateSkeletonPath( "c1301", path ) );
            ret.Add( "AuRa F", CalculateSkeletonPath( "c1401", path ) );
            ret.Add( "Hrothgar M", CalculateSkeletonPath( "c1501", path ) );
            // 1601 coming soon (tm)
            ret.Add( "Viera M", CalculateSkeletonPath( "c1701", path ) );
            ret.Add( "Viera F", CalculateSkeletonPath( "c1801", path ) );

            return ret;
        }
    }
}
