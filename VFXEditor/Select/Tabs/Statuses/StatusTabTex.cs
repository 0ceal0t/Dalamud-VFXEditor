using Lumina.Excel.GeneratedSheets;
using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select.Tabs.Statuses {
    public class StatusTabTex : SelectTab<StatusRow> {
        public StatusTabTex( SelectDialog dialog, string name ) : base( dialog, name, "Status-Tex", SelectResultType.GameStatus ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            var path = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.None );
            var hdPath = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.HiRes );

            DrawPath( "Icon", path, "", Selected.Name );
            if( Dalamud.DataManager.FileExists( hdPath ) ) {
                DrawPath( "HD Icon", hdPath, "", $"{Selected.Name} HD" );
            }
        }

        protected override string GetName( StatusRow item ) => item.Name;
    }
}