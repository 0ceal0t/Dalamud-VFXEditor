using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Bgm {
    public class BgmTab : SelectTab<BgmRow> {
        public BgmTab( SelectDialog dialog, string name ) : base( dialog, name, "Bgm" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idToTitle = new Dictionary<int, string>();
            try {
                fastCSV.ReadFile<object>( SelectDataUtils.OrchestrionPath, true, ',', ( _, columns ) => {
                    if( int.TryParse( columns[0], out var id ) ) idToTitle[id] = columns[1];
                    return false;
                } );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error reading csv {SelectDataUtils.OrchestrionPath}" );
            }

            var sheet = Dalamud.DataManager.GetExcelSheet<BGM>().Where( x => !string.IsNullOrEmpty( x.File.ExtractText() ) );
            foreach( var item in sheet ) Items.Add( new BgmRow( item, idToTitle ) );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameMusic );
        }
    }
}