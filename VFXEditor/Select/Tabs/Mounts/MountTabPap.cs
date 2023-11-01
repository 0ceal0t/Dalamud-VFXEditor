using ImGuiNET;
using OtterGui;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Mounts {
    public class MountTabPap : MountTab<Dictionary<string, Dictionary<string, string>>> {
        public MountTabPap( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( MountRow item, out Dictionary<string, Dictionary<string, string>> loaded ) {
            loaded = new();
            foreach( var (value, idx) in item.GetSeatPaps().WithIndex() ) {
                loaded.Add( $"Seat {idx + 1}", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( value ) ) );
            }
        }

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            DrawPath( "Mount", Selected.Pap, $"{Selected.Name} Mount", false );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            DrawWithHeader( Loaded, Selected.Name );
        }
    }
}