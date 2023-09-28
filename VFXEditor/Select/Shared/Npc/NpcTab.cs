using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VfxEditor.Select.Shared.Npc {
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

    public abstract class NpcTab : SelectTab<NpcRow, List<string>> {
        // Shared across multiple dialogs
        private static Dictionary<string, NpcFilesStruct> NpcFiles = new();

        public NpcTab( SelectDialog dialog, string name, SelectResultType resultType ) : base( dialog, name, "Shared-Npc", resultType ) { }

        public NpcTab( SelectDialog dialog, string name ) : this( dialog, name, SelectResultType.GameNpc ) { }

        // ===== LOADING =====

        public override void LoadData() {
            // maps uint ids to npc names
            var nameToString = NameToString;

            // https://raw.githubusercontent.com/ffxiv-teamcraft/ffxiv-teamcraft/staging/libs/data/src/lib/json/gubal-bnpcs-index.json

            var baseToName = JsonConvert.DeserializeObject<Dictionary<string, uint>>( File.ReadAllText( SelectDataUtils.BnpcPath ) );
            var battleNpcSheet = Dalamud.DataManager.GetExcelSheet<BNpcBase>();
            foreach( var entry in baseToName ) {
                if( !nameToString.TryGetValue( entry.Value, out var name ) ) continue;

                var bnpcRow = battleNpcSheet.GetRow( uint.Parse( entry.Key ) );
                if( !BnpcValid( bnpcRow ) ) continue;

                Items.Add( new NpcRow( bnpcRow.ModelChara.Value, name ) );
            }

            NpcFiles = JsonConvert.DeserializeObject<Dictionary<string, NpcFilesStruct>>( File.ReadAllText( SelectDataUtils.NpcFilesPath ) );
        }

        public override void LoadSelection( NpcRow item, out List<string> loaded ) {
            var files = NpcFiles.TryGetValue( item.ModelString, out var paths ) ? paths : new NpcFilesStruct();
            GetLoadedFiles( files, out loaded );
        }

        protected abstract void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded );

        // ===== DRAWING ======

        protected override bool CheckMatch( NpcRow item, string searchInput ) =>
            SelectUiUtils.Matches( item.Name, searchInput ) || SelectUiUtils.Matches( item.ModelString, searchInput );

        protected override void DrawExtra() => SelectUiUtils.NpcThankYou();

        protected override string GetName( NpcRow item ) => item.Name;

        // ====== UTILS ===========

        public static Dictionary<uint, string> NameToString => Dalamud.DataManager.GetExcelSheet<BNpcName>()
                .Where( x => !string.IsNullOrEmpty( x.Singular ) )
                .ToDictionary(
                    x => x.RowId,
                    x => x.Singular.ToString()
                );

        public static bool BnpcValid( BNpcBase bnpcRow ) {
            if( bnpcRow == null || bnpcRow.ModelChara.Value == null || bnpcRow.ModelChara.Value.Model == 0 ) return false;
            if( bnpcRow.ModelChara.Value.Type != 2 && bnpcRow.ModelChara.Value.Type != 3 ) return false;
            return true;
        }
    }
}
