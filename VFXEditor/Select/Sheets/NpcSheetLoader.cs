using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {

        public Dictionary<int, string> NpcIdToName = new();
        public Dictionary<string, List<string>> MonsterVfx = new();

        public class NpcCsvRow {
            public int Id;
            public string Name;
        }

        public override void OnLoad() {
            fastCSV.ReadFile<NpcCsvRow>( SheetManager.NpcCsv, true, ',', ( o, c ) => {
                o.Id = int.Parse( c[0] );
                o.Name = c[1];
                NpcIdToName[o.Id] = o.Name;
                return true;
            } );

            var sheet = SheetManager.DataManager.GetExcelSheet<ModelChara>().Where( x => ( x.Model != 0 && ( x.Type == 2 || x.Type == 3 ) ) );
            foreach( var item in sheet ) {
                var i = new XivNpc( item, NpcIdToName );
                if( i.CSV_Defined ) {
                    Items.Add( i );
                }
            }

            var text = File.ReadAllText( SheetManager.MonsterJson );
            MonsterVfx = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>( text );
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            selectedItem = new XivNpcSelected( item, MonsterVfx.TryGetValue( item.Id, out var paths ) ? paths : new List<string>() );

            return true;
        }
    }
}
