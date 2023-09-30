using OtterGui;
using System.IO;
using System.Linq;
using VfxEditor.Select.Shared.Common;

namespace VfxEditor.Select.Shpk.Common {
    public class CommonTab : SelectTab<CommonRow> {
        public CommonTab( SelectDialog dialog, string name ) : base( dialog, name, "Shpk-Common", SelectResultType.GameUi ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var (line, idx) in File.ReadLines( SelectDataUtils.MiscShpkPath ).Where( x => !string.IsNullOrEmpty( x ) ).WithIndex() ) {
                Items.Add( new CommonRow( idx, line, line.Replace( ".shpk", "" ).Replace( "shader/", "" ), 0 ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "DX9", Selected.Path, $"{Selected.Name} (DX9)" );
            DrawPath( "DX11", Selected.Path.Replace( "shader/", "shader/sm5/" ), $"{Selected.Name} (DX11)" );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}
