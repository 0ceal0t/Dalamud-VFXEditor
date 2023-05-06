using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Pap.Mount {
    public class MountTab : SelectTab<MountRow, Dictionary<string, Dictionary<string, string>>> {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Mount" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        public override void LoadSelection( MountRow item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = new();

            var papPaths = item.GetMountPaps();
            for( var i = 0; i <  papPaths.Count; i++ ) {
                loaded.Add( $"Seat {i + 1}", SelectUtils.FileExistsFilter( SelectUtils.GetAllSkeletonPaths( papPaths[i] ) ) );
            }
        }

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            Dialog.DrawPapsWithHeader( Loaded, SelectResultType.GameNpc, Selected.Name, parentId );
        }

        protected override string GetName( MountRow item ) => item.Name;
    }
}
