using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select.Tex.Action {
    public class ActionTab : SelectTab<ActionRow> {
        public ActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Tex-Action", SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
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

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
