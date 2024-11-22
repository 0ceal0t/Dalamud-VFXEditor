using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabScd : SelectTab<ActionRow, ParsedPaths> {
        public ActionTabScd( SelectDialog dialog, string name ) : base( dialog, name, "Action-Scd" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>().Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        public override void LoadSelection( ActionRow item, out ParsedPaths loaded ) {
            var paths = new List<string>();

            if( !string.IsNullOrEmpty( item.CastVfxPath ) ) paths.Add( item.CastVfxPath );
            if( !string.IsNullOrEmpty( item.StartVfxPath ) ) paths.Add( item.StartVfxPath );

            PopulatePaths( item.StartTmbPath, paths );
            PopulatePaths( item.EndTmbPath, paths );
            PopulatePaths( item.HitTmbPath, paths );

            ParsedPaths.ReadFile( paths, SelectDataUtils.ScdRegex, out loaded );
        }

        private static void PopulatePaths( string tmbPath, List<string> paths ) {
            if( string.IsNullOrEmpty( tmbPath ) ) return;
            paths.Add( tmbPath );

            ParsedPaths.ReadFile( tmbPath, SelectDataUtils.AvfxRegex, out var parsedAvfx );
            if( parsedAvfx == null ) return;

            paths.AddRange( parsedAvfx.Paths );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded.Paths, Selected.Name, SelectResultType.GameAction );
        }
    }
}