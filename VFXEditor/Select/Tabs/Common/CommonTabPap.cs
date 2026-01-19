using System.IO;
using System.Linq;

namespace VfxEditor.Select.Tabs.Common {
    public class CommonTabPap : SelectTab<CommonRow> {
        public CommonTabPap( SelectDialog dialog, string name ) : base( dialog, name, "Common-Pap" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idx = 0;
            foreach( var line in File.ReadLines( SelectDataUtils.CommonPapPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                if( !( line.Contains( "bt_common/event/" ) || line.Contains( "bt_common/event_base/" ) ) ) {
                    Items.Add( new CommonRow( idx++, line, line.Replace( "chara/human/", "" ).Replace( "/animation/", ", " ).Replace( "/bt_common/", " -  " ).Replace( "/bt_", " -  bt_" ).Replace( ".pap", "" ), 0 ) );
                }
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameMisc );
        }
    }
}
