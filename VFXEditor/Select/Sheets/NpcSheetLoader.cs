using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {

        public Dictionary<int, string> NpcIdToName = new();
        public Dictionary<string, List<string>> MonsterVfx = new();

        public NpcSheetLoader( SheetManager manager, DalamudPluginInterface pluginInterface ) : base( manager, pluginInterface ) {
        }

        public class NpcCsvRow {
            public int Id;
            public string Name;
        }

        public override void OnLoad() {
            fastCSV.ReadFile<NpcCsvRow>( Manager.NpcCsv, true, ',', ( o, c ) => {
                o.Id = int.Parse( c[0] );
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

            var text = File.ReadAllText( Manager.MonsterJson );
            MonsterVfx = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>( text );
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            selectedItem = new XivNpcSelected( item, MonsterVfx.TryGetValue( item.Id, out var paths ) ? paths : new List<string>() );

            return true;
        }
    }
}
