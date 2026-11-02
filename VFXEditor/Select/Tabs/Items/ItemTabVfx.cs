using Dalamud.Bindings.ImGui;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Interop.Penumbra;

namespace VfxEditor.Select.Tabs.Items {
    public class SelectedVfx {
        public string ImcPath;
        public List<string> Paths;
    }

    public class ItemTabVfx : ItemTab<SelectedVfx> {
        public ItemTabVfx( SelectDialog dialog, string name ) : this( dialog, name, "Item-Vfx", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor ) { }

        public ItemTabVfx( SelectDialog dialog, string name, string state, ItemTabFilter filter ) : base( dialog, name, state, filter ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out SelectedVfx loaded ) {
            loaded = null;
            var imcPath = item.ImcPath;
            if( !Dalamud.DataManager.FileExists( imcPath ) ) return;
            try {
                var file = Dalamud.DataManager.GetFile<ImcFile>( imcPath );
                var ids = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();

                var manipulations = Plugin.PenumbraIpc.GetPlayerImcEntries();
                foreach( var (identifier, entry) in manipulations ) {
                    if( identifier.PrimaryId == item.Ids.Id && identifier.Variant == item.Variant && $"{item.Type}" == $"{identifier.EquipSlot}" ) ids.Add( entry.VfxId );
                }

                loaded = new() {
                    ImcPath = file.FilePath,
                    Paths = [.. ids.Select( item.GetVfxPath )]
                };
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error loading IMC file {imcPath}" );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            ImGui.Text( $"Variant: {Selected.Variant}" );
            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Loaded.ImcPath );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Dialog.DrawPaths( Loaded.Paths, Selected.Name, SelectResultType.GameItem );

            if( Loaded.Paths.Count == 0 ) SelectUiUtils.DisplayNoVfx();
        }
    }
}