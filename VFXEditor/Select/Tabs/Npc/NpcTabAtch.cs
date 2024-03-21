using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Select.Tabs.Npc {
    public class NpcTabAtch : SelectTab<NpcRow> {
        public NpcTabAtch( SelectDialog dialog, string name ) : base( dialog, name, "Npc-Atch" ) { }

        public override void LoadData() {
            var file = Dalamud.DataManager.GetFile( "chara/xls/attachoffset/attachoffsetexist.waoe" );
            using var ms = new MemoryStream( file.Data );
            using var reader = new BinaryReader( ms );

            var count = reader.ReadUInt16() + 1;
            var offsetsExist = new HashSet<ushort>();
            for( var i = 0; i < count; i++ ) offsetsExist.Add( reader.ReadUInt16() );

            // ================

            var nameToString = NpcTab.NameToString;

            var baseToName = JsonConvert.DeserializeObject<Dictionary<string, uint>>( File.ReadAllText( SelectDataUtils.BnpcPath ) );
            var battleNpcSheet = Dalamud.DataManager.GetExcelSheet<BNpcBase>();
            foreach( var entry in baseToName ) {
                if( !nameToString.TryGetValue( entry.Value, out var name ) ) continue;

                var bnpcRow = battleNpcSheet.GetRow( uint.Parse( entry.Key ) );
                if( !NpcTab.BnpcValid( bnpcRow ) ) continue;

                var modelChara = bnpcRow.ModelChara.Value;
                var id = ( ushort )( modelChara.Type == 2 ? modelChara.Model + 10000 : modelChara.Model );
                if( !offsetsExist.Contains( id ) ) continue;

                Items.Add( new NpcRow( modelChara, name ) );
            }

        }

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.AtchPath, Selected.Name, SelectResultType.GameNpc );
        }

        protected override string GetName( NpcRow item ) => item.Name;
    }
}