using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select.Tabs.Statuses {
    public class StatusTabTex : SelectTab<StatusRow> {
        public StatusTabTex( SelectDialog dialog, string name ) : base( dialog, name, "Status-Tex" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            var icon = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.None );
            var hd = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.HiRes );

            var paths = new Dictionary<string, string>() {
                { "Icon", icon }
            };
            if( Dalamud.DataManager.FileExists( hd ) ) paths["HD Icon"] = hd;

            Dialog.DrawPaths( paths, Selected.Name, SelectResultType.GameStatus );
        }
    }
}