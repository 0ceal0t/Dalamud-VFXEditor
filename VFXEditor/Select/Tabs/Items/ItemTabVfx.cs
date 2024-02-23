using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Items {
    public class SelectedVfx {
        public string ImcPath;
        public List<string> Paths;
    }

    public class ItemTabVfx : ItemTab<SelectedVfx> {
        public ItemTabVfx( SelectDialog dialog, string name ) : base( dialog, name, "Item-Vfx", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out SelectedVfx loaded ) {
            loaded = null;
            var imcPath = item.ImcPath;
            if( !Dalamud.DataManager.FileExists( imcPath ) ) return;
            try {
                var file = Dalamud.DataManager.GetFile<ImcFile>( imcPath );
                var ids = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();

                loaded = new() {
                    ImcPath = file.FilePath,
                    Paths = ids.Select( item.GetVfxPath ).ToList()
                };
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading IMC file " + imcPath );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            ImGui.Text( $"Variant: {Selected.Variant}" );
            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Loaded.ImcPath );

            DrawPaths( "VFX", Loaded.Paths, Selected.Name );

            if( Loaded.Paths.Count == 0 ) SelectUiUtils.DisplayNoVfx();
        }
    }
}
