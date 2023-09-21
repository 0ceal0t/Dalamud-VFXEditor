using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared.Common;

namespace VfxEditor.Select.Scd.Common {
    public class CommonTab : SelectTab<CommonRow> {
        public CommonTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Common", SelectResultType.GameMisc ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var lineIdx = 0;
            Items.AddRange( new List<CommonRow>() {
                new( lineIdx++, "sound/system/SE_UI.scd", "SE_UI", 0 ),
                new( lineIdx++, "sound/vfx/SE_VFX_common.scd", "SE_VFX_common", 0 ),
                new( lineIdx++, "sound/strm/gaya_fate_01.scd", "Gaya_Fate_01", 0 ),
                new( lineIdx++, "sound/strm/gaya_lestarea_01.scd", "Gaya_Lestarea_01", 0 ),
                new( lineIdx++, "sound/strm/gaya_village_01.scd", "Gaya_Village_01", 0 ),
            } );

            foreach( var line in Dalamud.DataManager.GetExcelSheet<Jingle>().Where( x => !string.IsNullOrEmpty( x.Name ) ) ) {
                var name = line.Name.ToString();
                var path = name.StartsWith( "/" ) ? $"sound{line.Name.ToString().ToLower()}.scd" : $"sound/zingle/zingle_{line.Name.ToString().ToLower()}.scd";
                Items.Add( new( lineIdx++, path, name, 0 ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Path", Selected.Path, Selected.Name, true );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}
