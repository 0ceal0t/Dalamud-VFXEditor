using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {

        public Dictionary<int, string> NpcIdToName = new();
        public NpcSheetLoader( SheetManager manager, DalamudPluginInterface pluginInterface ) : base( manager, pluginInterface ) {
        }

        public class NpcCsvRow {
            public int Id;
            public string Name;
        }

        public override void OnLoad() {
            fastCSV.ReadFile<NpcCsvRow>( Manager.NpcCsv, true, ',', ( o, c ) => {
                o.Id = Int32.Parse( c[0] );
                o.Name = c[1];
                NpcIdToName[o.Id] = o.Name;
                return true;
            } );

            var sheet = PluginInterface.Data.GetExcelSheet<ModelChara>().Where( x => ( x.Model != 0 && ( x.Type == 2 || x.Type == 3 ) ) );
            foreach( var item in sheet ) {
                var i = new XivNpc( item, NpcIdToName );
                if( i.CSV_Defined ) {
                    Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            selectedItem = null;
            var imcPath = item.GetImcPath();
            var result = PluginInterface.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    var tmbPath = item.GetTmbPath();
                    var files = new List<Lumina.Data.FileResource>();
                    for( var spIdx = 1; spIdx < 35; spIdx++ ) {
                        var mainTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + ".tmb";
                        var hitTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + "_hit.tmb";
                        if( PluginInterface.Data.FileExists( mainTmb ) ) {
                            files.Add( PluginInterface.Data.GetFile( mainTmb ) );
                        }
                        if( PluginInterface.Data.FileExists( hitTmb ) ) {
                            files.Add( PluginInterface.Data.GetFile( hitTmb ) );
                        }
                    }
                    selectedItem = new XivNpcSelected( file, item, files );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error reading npc file", e );
                    return false;
                }
            }
            return result;
        }
    }
}
