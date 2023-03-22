using System.Linq;
using System.IO;

namespace VfxEditor.Select.Tmb.Common {
    public class CommonTab : SelectTab<CommonRow> {
        public CommonTab( SelectDialog dialog, string name ) : base( dialog, name, "Tmb-Common" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var lineIdx = 0;
            foreach( var line in File.ReadLines( SelectUtils.MiscTmbPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                Items.Add( new CommonRow( lineIdx, line, line.Replace( "chara/action/", "" ).Replace( ".tmb", "" ) ) );
                lineIdx++;
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameAction, Selected.Name, true );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}
