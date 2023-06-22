using Dalamud.Logging;
using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Linq;
using VfxEditor.Select.Shared.Mount;

namespace VfxEditor.Select.Vfx.Mount {
    public class MountRowSelected {
        public string ImcPath;
        public int VfxId;
        public string VfxPath;
    }

    public class MountTab : SelectTab<MountRow, MountRowSelected> {
        public MountTab( SelectDialog dialog, string name ) : base( dialog, name, "Shared-Mount" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) Items.Add( new MountRow( item ) );
        }

        public override void LoadSelection( MountRow item, out MountRowSelected loaded ) {
            loaded = null;
            var imcPath = item.GetImcPath();
            if( Plugin.DataManager.FileExists( imcPath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile<ImcFile>( imcPath );
                    var vfxIds = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
                    var vfxId = vfxIds.Count > 0 ? vfxIds[0] : 0;

                    loaded = new() {
                        ImcPath = file.FilePath,
                        VfxId = vfxId,
                        VfxPath = vfxId > 0 ? item.GetVfxPath( vfxId ) : ""
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

            ImGui.Text( "Variant: " + Selected.Variant );
            ImGui.Text( "IMC: " );

            ImGui.SameLine();
            SelectTabUtils.DisplayPath( Loaded.ImcPath );

            Dialog.DrawPath( "VFX", Loaded.VfxPath, SelectResultType.GameNpc, Selected.Name, true );
        }

        protected override string GetName( MountRow item ) => item.Name;
    }
}
