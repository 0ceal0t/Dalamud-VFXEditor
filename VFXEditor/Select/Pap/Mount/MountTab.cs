using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Pap.Mount {
    public class MountTab : SelectTab<MountRow, Dictionary<string, Dictionary<string, string>>> {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Mount", SelectResultType.GameMount ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        public override void LoadSelection( MountRow item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = [];

            var papPaths = item.GetMountSeatPaps();
            for( var i = 0; i < papPaths.Count; i++ ) {
                loaded.Add( $"Seat {i + 1}", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( papPaths[i] ) ) );
            }
        }

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            DrawPath( "Mount", Selected.GetMountPap(), $"{Selected.Name} Mount", false );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            DrawPapsWithHeader( Loaded, Selected.Name );
        }

        protected override string GetName( MountRow item ) => item.Name;
    }
}
