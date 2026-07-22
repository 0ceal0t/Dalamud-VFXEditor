using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace VfxEditor.Select.Tabs.NpcID {
    public struct NpcFilesStruct {
        public List<string> vfx;
        public List<string> tmb;
        public List<string> pap;

        public NpcFilesStruct() {
            vfx = [];
            tmb = [];
            pap = [];
        }
    }

    public abstract class NpcTab : SelectTab<NpcRow, List<string>> {
        private static Dictionary<string, NpcFilesStruct> NpcFiles = [];

        private readonly SelectResultType ResultType;

        public NpcTab( SelectDialog dialog, string name, SelectResultType resultType ) : base( dialog, name, "Npc" ) {
            ResultType = resultType;
        }

        public NpcTab( SelectDialog dialog, string name ) : this( dialog, name, SelectResultType.GameNpc ) { }

        // ===== LOADING =====

        public override void LoadData() {
            NpcFiles = JsonConvert.DeserializeObject<Dictionary<string, NpcFilesStruct>>( File.ReadAllText( SelectDataUtils.NpcFilesPath ) );

            foreach( var item in NpcFiles.OrderBy( r => r.Key ) ){
                Items.Add( new NpcRow( item.Key ) );
            }
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

        protected override void DrawSelected() {
            Dialog.DrawPaths( Loaded, Selected.Name, ResultType );
        }

        // ====== UTILS ===========

    }
}