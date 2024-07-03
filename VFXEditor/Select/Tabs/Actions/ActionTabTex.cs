using Lumina.Excel.GeneratedSheets2;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabTex : SelectTab<ActionRow> {
        public ActionTabTex( SelectDialog dialog, string name ) : base( dialog, name, "Action-Tex" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            var path = Dalamud.TextureProvider.GetIconPath( ( uint )Selected.Icon );
            var hdPath = Dalamud.TextureProvider.GetIconPath( new( Selected.Icon, hiRes: true ) );

            var paths = new Dictionary<string, string> {
                { "Icon", path }
            };
            if( Dalamud.DataManager.FileExists( hdPath ) ) paths.Add( "HD Icon", hdPath );
            Dialog.DrawPaths( paths, Selected.Name, SelectResultType.GameAction );
        }
    }
}