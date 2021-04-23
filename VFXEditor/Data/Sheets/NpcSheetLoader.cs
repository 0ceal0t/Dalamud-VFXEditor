using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {

        public Dictionary<int, string> NpcIdToName = new Dictionary<int, string>();
        public NpcSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
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

            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<ModelChara>().Where( x => ( x.Model != 0 && ( x.Type == 2 || x.Type == 3 ) ) );
            foreach( var item in _sheet ) {
                var i = new XivNpc( item, NpcIdToName );
                if( i.CSV_Defined ) {
                    Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _plugin.PluginInterface.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    var tmbPath = item.GetTmbPath();
                    List<Lumina.Data.FileResource> files = new List<Lumina.Data.FileResource>();
                    for( int spIdx = 1; spIdx < 35; spIdx++ ) {
                        var mainTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + ".tmb";
                        var hitTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + "_hit.tmb";
                        if( _plugin.PluginInterface.Data.FileExists( mainTmb ) ) {
                            files.Add( _plugin.PluginInterface.Data.GetFile( mainTmb ) );
                        }
                        if( _plugin.PluginInterface.Data.FileExists( hitTmb ) ) {
                            files.Add( _plugin.PluginInterface.Data.GetFile( hitTmb ) );
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
