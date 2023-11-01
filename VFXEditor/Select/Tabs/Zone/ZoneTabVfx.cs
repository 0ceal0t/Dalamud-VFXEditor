using ImGuiNET;

namespace VfxEditor.Select.Tabs.Zone {
    public class ZoneTabVfx : ZoneTab<ParsedPaths> {
        public ZoneTabVfx( SelectDialog dialog, string name ) : base( dialog, name, "Zone-Vfx" ) { }

        // ===== LOADING =====

        public override void LoadSelection( ZoneRow item, out ParsedPaths loaded ) => ParsedPaths.ReadFile( item.LgbPath, SelectDataUtils.AvfxRegex, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            ImGui.Text( "LGB: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.LgbPath );

            DrawPaths( "VFX", Loaded.Paths, Selected.Name, true );
        }
    }
}