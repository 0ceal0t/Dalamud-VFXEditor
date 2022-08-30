using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class NpcSheetLoader : SheetLoader<XivNpc, XivNpcSelected> {
        public Dictionary<string, NpcFilesStruct> NpcFiles = new();

        public class NpcCsvRow {
            public int Id;
            public string Name;
        }

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

        public override void OnLoad() {
            // map N:0000000 -> "Moogle"
            var bNpcNames = VfxEditor.DataManager.GetExcelSheet<BNpcName>()
                .Where( x => !string.IsNullOrEmpty( x.Singular ) )
                .ToDictionary(
                    x => $"N:{x.RowId.ToString().PadLeft(7, '0')}",
                    x => x.Singular.ToString()
                );

            // map B:0000000 -> N:0000000000
            var npcNamesText = File.ReadAllText( SheetManager.NpcNamesPath );
            var npcNames = JsonConvert.DeserializeObject<Dictionary<string ,string>>( npcNamesText );

            // old method for keeping track of npc names, no longer updated
            // map modelId -> Name
            Dictionary<uint, string> oldBattleNpcNames = new();
            fastCSV.ReadFile<NpcCsvRow>( SheetManager.NpcNamesOldPath, true, ',', ( o, c ) => {
                o.Id = int.Parse( c[0] );
                o.Name = c[1];

                oldBattleNpcNames[(uint)o.Id] = o.Name;
                return true;
            } );

            HashSet<string> itemsAlreadyAdded = new();

            // map B:00000 -> "Moogle"
            var combinedNpcNames = npcNames.ToDictionary(
                x => x.Key,
                x => x.Value.StartsWith( "N:" ) ? (bNpcNames.TryGetValue(x.Value, out var resolvedName) ? resolvedName : x.Value) : x.Value 
            );

            // add battle npcs
            var battleNpcSheet = VfxEditor.DataManager.GetExcelSheet<BNpcBase>().Where( x => x.ModelChara.Value.Model != 0 && ( x.ModelChara.Value.Type == 2 || x.ModelChara.Value.Type == 3 ) );
            foreach( var item in battleNpcSheet ) {
                var id = $"B:{item.RowId.ToString().PadLeft( 7, '0' )}";

                AddNpcIfFound( item.ModelChara.Value, id, combinedNpcNames, oldBattleNpcNames, itemsAlreadyAdded );
            }

            // add event npcs
            var eventNpcSheet = VfxEditor.DataManager.GetExcelSheet<BNpcBase>().Where( x => x.ModelChara.Value.Model != 0 && ( x.ModelChara.Value.Type == 2 || x.ModelChara.Value.Type == 3 ) );
            foreach( var item in eventNpcSheet ) {
                var id = $"E:{item.RowId.ToString().PadLeft( 7, '0' )}";

                AddNpcIfFound( item.ModelChara.Value, id, combinedNpcNames, oldBattleNpcNames, itemsAlreadyAdded );
            }

            NpcFiles = JsonConvert.DeserializeObject<Dictionary<string, NpcFilesStruct>>( File.ReadAllText( SheetManager.NpcFilesPath ) );
        }

        private void AddNpcIfFound(ModelChara model, string id, Dictionary<string, string> combinedNpcNames, Dictionary<uint, string> oldBattleNpcNames, HashSet<string> itemsAlreadyAdded ) {
            var modelId = model.RowId;
            var foundName = combinedNpcNames.TryGetValue( id, out var resolvedValue ) ? resolvedValue :
                    ( oldBattleNpcNames.TryGetValue( modelId, out var resolvedValue2 ) ? resolvedValue2 : null );
            if( string.IsNullOrEmpty( foundName ) ) return;
            if( itemsAlreadyAdded.Contains( foundName ) ) return;
            itemsAlreadyAdded.Add( foundName );
            Items.Add( new XivNpc( model, foundName ) );
        }

        public override bool SelectItem( XivNpc item, out XivNpcSelected selectedItem ) {
            var files = NpcFiles.TryGetValue( item.Id, out var paths ) ? paths : new NpcFilesStruct();
            selectedItem = new XivNpcSelected( item, files.vfx, files.tmb, files.pap );
            return true;
        }
    }
}
