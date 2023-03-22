using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select2.Shared;
using VfxEditor.Select2.Shared.Zone;

namespace VfxEditor.Select2.Vfx.Zone {
    public class ZoneTab : SelectTab<ZoneRow, ParseAvfxFromFile> {
        public ZoneTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Zone" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new ZoneRow( item ) );
        }

        public override void LoadSelection( ZoneRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.LgbPath, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "LGB: " );
            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Selected.LgbPath );

            Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameZone, Selected.Name, true );
        }

        protected override string GetName( ZoneRow item ) => item.Name;
    }
}
