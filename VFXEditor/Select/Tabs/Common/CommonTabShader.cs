using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VfxEditor.Select.Tabs.Common {
    public class CommonTabShader : SelectTab<CommonRow> {
        private readonly string Path;
        private readonly string Extension;

        public CommonTabShader( SelectDialog dialog, string name, string stateId, string path, string extension ) : base( dialog, name, stateId, SelectResultType.GameUi ) {
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
            DrawPaths( new Dictionary<string, string>() {
                { "DX9", Selected.Path },
                { "DX11", Selected.Path.Replace( "shader/", "shader/sm5/" ) }
            }, Selected.Name );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}