using Dalamud.Bindings.ImGui;
using Lumina.Data.Files;
using System;
using System.Linq;

namespace VfxEditor.Select.Tabs.Mounts {
    public class SelectedVfx {
        public string ImcPath;
        public int Id;
        public string Path;
    }

    public class MountTabVfx : MountTab<SelectedVfx> {
        public MountTabVfx( SelectDialog dialog, string name ) : base( dialog, name ) { }

        // ===== LOADING =====

        public override void LoadSelection( MountRow item, out SelectedVfx loaded ) {
            loaded = null;
            var imcPath = item.ImcPath;

            if( !Dalamud.DataManager.FileExists( imcPath ) ) return;
            try {
                var file = Dalamud.DataManager.GetFile<ImcFile>( imcPath );
                var ids = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
                var id = ids.Count > 0 ? ids[0] : 0;

                loaded = new() {
                    ImcPath = file.FilePath,
                    Id = id,
                    Path = id > 0 ? item.GetVfxPath( id ) : ""
                };
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading IMC file " + imcPath );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            ImGui.TextDisabled( "Variant: " + Selected.Variant );
            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Loaded.ImcPath );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Dialog.DrawPaths( Loaded.Path, Selected.Name, SelectResultType.GameMount );
        }
    }
}