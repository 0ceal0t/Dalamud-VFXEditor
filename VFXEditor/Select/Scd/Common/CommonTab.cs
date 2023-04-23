using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Scd.Common {
    public class CommonTab : SelectTab<CommonRow> {
        public CommonTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Common" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var lineIdx = 0;
            Items.AddRange( new List<CommonRow>() {
                new( lineIdx++, "sound/system/SE_UI.scd", "SE_UI" ),
                new( lineIdx++, "sound/vfx/SE_VFX_common.scd", "SE_VFX_common" ),
                new( lineIdx++, "sound/strm/gaya_fate_01.scd", "Gaya_Fate_01" ),
                new( lineIdx++, "sound/strm/gaya_lestarea_01.scd", "Gaya_Lestarea_01" ),
                new( lineIdx++, "sound/strm/gaya_village_01.scd", "Gaya_Village_01" ),
            } );

            foreach( var line in Plugin.DataManager.GetExcelSheet<Jingle>().Where( x => !string.IsNullOrEmpty( x.Name ) ) ) {
                Items.Add( new( lineIdx++, $"sound/zingle/zingle_{line.Name.ToString().ToLower()}.scd", line.Name.ToString() ) );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            Dialog.DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameAction, Selected.Name, true );
        }

        protected override string GetName( CommonRow item ) => item.Name;
    }
}
