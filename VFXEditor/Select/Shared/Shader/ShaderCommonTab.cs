using OtterGui;
using System.IO;
using System.Linq;
using VfxEditor.Select.Shared.Common;

namespace VfxEditor.Select.Shared.Shader {
    public class ShaderCommonTab : SelectTab<CommonRow> {
        private readonly string Path;
        private readonly string Extension;

        public ShaderCommonTab( SelectDialog dialog, string state, string path, string extension ) : base( dialog, "Common", state, SelectResultType.GameUi ) {
            Path = path;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var (line, idx) in File.ReadLines( Path ).Where( x => !string.IsNullOrEmpty( x ) ).WithIndex() ) {
                Items.Add( new CommonRow( idx, line, line.Replace( Extension, "" ).Replace( "shader/", "" ), 0 ) );
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
