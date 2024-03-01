using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabScd : SelectTab<ActionRow, ParsedPaths> {
        public ActionTabScd( SelectDialog dialog, string name ) : base( dialog, name, "Action-Scd", SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new ActionRow( item ) );
        }

        public override void LoadSelection( ActionRow item, out ParsedPaths loaded ) {
            var paths = new List<string>();

            if( !string.IsNullOrEmpty( item.CastVfxPath ) ) paths.Add( item.CastVfxPath );
            if( !string.IsNullOrEmpty( item.StartVfxPath ) ) paths.Add( item.StartVfxPath );

            PopulatePaths( item.StartPath, paths );
            PopulatePaths( item.EndPath, paths );
            PopulatePaths( item.HitPath, paths );

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
            DrawPaths( Loaded.Paths, Selected.Name );
        }

        protected override string GetName( ActionRow item ) => item.Name;

        protected override uint GetIconId( ActionRow item ) => item.Icon;
    }
}