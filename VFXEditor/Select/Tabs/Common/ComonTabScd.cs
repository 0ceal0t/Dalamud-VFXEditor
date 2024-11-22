using Lumina.Excel.Sheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Common {
    public class CommonTabScd : SelectTab<CommonRow> {
        public CommonTabScd( SelectDialog dialog, string name ) : base( dialog, name, "Common-Scd" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var idx = 0;
            Items.AddRange( [
                new( idx++, "sound/system/SE_UI.scd", "SE_UI", 0 ),
                new( idx++, "sound/vfx/SE_VFX_common.scd", "SE_VFX_common", 0 ),
                new( idx++, "sound/strm/gaya_fate_01.scd", "Gaya_Fate_01", 0 ),
                new( idx++, "sound/strm/gaya_lestarea_01.scd", "Gaya_Lestarea_01", 0 ),
                new( idx++, "sound/strm/gaya_village_01.scd", "Gaya_Village_01", 0 ),
            ] );

            foreach( var line in Dalamud.DataManager.GetExcelSheet<Jingle>().Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) ) ) {
                var name = line.Name.ToString();
                var path = name.StartsWith( '/' ) ? $"sound{line.Name.ToString().ToLower()}.scd" : $"sound/zingle/zingle_{line.Name.ToString().ToLower()}.scd";
                Items.Add( new( idx++, path, name, 0 ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPaths( Selected.Path, Selected.Name, SelectResultType.GameMisc );
        }
    }
}