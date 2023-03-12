using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {
        public Dictionary<string, NpcFilesStruct> NpcFiles = new();

        public struct NpcFilesStruct {
            public List<string> vfx;
            public List<string> tmb;
            public List<string> pap;

            public NpcFilesStruct() {
                vfx = new();
                tmb = new();
                pap = new();
            }
        }

        public struct BnpcStruct {
            public uint bnpcBase;
            public uint bnpcName;
        }

        public override void OnLoad() {
            // maps uint ids to npc names
            var nameToString = Plugin.DataManager.GetExcelSheet<BNpcName>()
                .Where( x => !string.IsNullOrEmpty( x.Singular ) )
                .ToDictionary(
                    x => x.RowId,
                    x => x.Singular.ToString()
                );

            var baseToName = JsonConvert.DeserializeObject<Dictionary<string, List<BnpcStruct>>>( File.ReadAllText( SheetManager.BnpcPath ) )["bnpc"];

            var battleNpcSheet = Plugin.DataManager.GetExcelSheet<BNpcBase>();

            foreach( var entry in baseToName ) {
                if( !nameToString.TryGetValue( entry.bnpcName, out var name ) ) continue;
                var bnpcRow = battleNpcSheet.GetRow( entry.bnpcBase );
                if( bnpcRow == null || bnpcRow.ModelChara.Value == null || bnpcRow.ModelChara.Value.Model == 0 ) continue;
                if( bnpcRow.ModelChara.Value.Type != 2 && bnpcRow.ModelChara.Value.Type != 3 ) continue;
                Items.Add( new XivNpc( bnpcRow.ModelChara.Value, name ) );
            }

            NpcFiles = JsonConvert.DeserializeObject<Dictionary<string, NpcFilesStruct>>( File.ReadAllText( SheetManager.NpcFilesPath ) );
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            var files = NpcFiles.TryGetValue( item.Id, out var paths ) ? paths : new NpcFilesStruct();
            selectedItem = new XivNpcSelected( item, files.vfx, files.tmb, files.pap );
            return true;
        }
    }
}
