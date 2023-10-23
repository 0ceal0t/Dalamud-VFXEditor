using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Scd.Action {
    public class ActionTab : SelectTab<ActionRow, ParsedPaths> {
        public ActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Action", SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) );
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
            DrawIcon( Selected.Icon );
            DrawPaths( "Sound", Loaded.Paths, Selected.Name, true );
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
