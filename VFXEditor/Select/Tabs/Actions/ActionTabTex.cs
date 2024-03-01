using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabTex : SelectTab<ActionRow> {
        public ActionTabTex( SelectDialog dialog, string name ) : base( dialog, name, "Action-Tex", SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            var path = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.None );
            var hdPath = Dalamud.TextureProvider.GetIconPath( Selected.Icon, IconFlags.HiRes );

            var paths = new Dictionary<string, string> {
                { "Icon", path }
            };
            if( Dalamud.DataManager.FileExists( hdPath ) ) paths.Add( "HD Icons", hdPath );
            DrawPaths( paths, Selected.Name );
        }

        protected override string GetName( ActionRow item ) => item.Name;

        protected override uint GetIconId( ActionRow item ) => item.Icon;
    }
}