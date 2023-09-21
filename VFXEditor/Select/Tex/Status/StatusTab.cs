using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select.Tex.Status {
    public class StatusTab : SelectTab<StatusRow> {
        public StatusTab( SelectDialog dialog, string name ) : base( dialog, name, "Tex-Status", SelectResultType.GameStatus ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            var path = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.None );
            var hdPath = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.HiRes );

            DrawPath( "Icon", path, "", $"{Selected.Name}" );
            if( Dalamud.DataManager.FileExists( hdPath ) ) {
                DrawPath( "HD Icon", hdPath, "", $"{Selected.Name} HD" );
            }
        }

        protected override string GetName( StatusRow item ) => item.Name;
    }
}
