using System.IO;
using System.Linq;

namespace VfxEditor.Select.Tabs.Common {
    public class CommonTabTmb : SelectTab<CommonRow> {
        public CommonTabTmb( SelectDialog dialog, string name ) : base( dialog, name, "Common-Tmb", SelectResultType.GameMisc ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idx = 0;
            foreach( var line in File.ReadLines( SelectDataUtils.CommonTmbPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                Items.Add( new CommonRow( idx++, line, line.Replace( "chara/action/", "" ).Replace( ".tmb", "" ), 0 ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Path", Selected.Path, Selected.Name, true );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}