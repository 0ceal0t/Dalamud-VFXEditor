using Dalamud.Logging;
using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Vfx.Item {
    public class ItemRowSelected {
        public string ImcPath;
        public List<string> VfxPaths;
    }

    public class VfxItemTab : ItemTab<ItemRowSelected> {
        public VfxItemTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out ItemRowSelected loaded ) {
            loaded = null;
            var imcPath = item.GetImcPath();
            if( Plugin.DataManager.FileExists( imcPath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile<ImcFile>( imcPath );
                    var vfxIds = file.GetParts().Select( x => x.Variants[item.GetVariant() - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();

                    loaded = new() {
                        ImcPath = file.FilePath,
                        VfxPaths = vfxIds.Select( item.GetVfxPath ).ToList()
                    };
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error loading IMC file " + imcPath );
                }
            }
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectTabUtils.DrawIcon( Icon );

            ImGui.Text( "Variant: " + Selected.GetVariant() );
            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Loaded.ImcPath );

            Dialog.DrawPaths( "VFX", Loaded.VfxPaths, SelectResultType.GameItem, Selected.Name, true );

            if( Loaded.VfxPaths.Count == 0 ) SelectTabUtils.DisplayNoVfx();
        }
    }
}
