using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.FileBrowser;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Dialogs {
    public class AvfxExport {
        private readonly List<AvfxExportCategory> Categories;
        private bool ExportDependencies = true;

        public AvfxExport( AvfxFile file ) {
            Categories = [
                new ExportDialogCategory<AvfxTimeline>( file.NodeGroupSet.Timelines, "Timelines" ),
                new ExportDialogCategory<AvfxEmitter>( file.NodeGroupSet.Emitters, "Emitters" ),
                new ExportDialogCategory<AvfxParticle>( file.NodeGroupSet.Particles, "Particles" ),
                new ExportDialogCategory<AvfxEffector>( file.NodeGroupSet.Effectors, "Effectors" ),
                new ExportDialogCategory<AvfxBinder>( file.NodeGroupSet.Binders, "Binders" ),
                new ExportDialogCategory<AvfxTexture>( file.NodeGroupSet.Textures, "Textures" ),
                new ExportDialogCategory<AvfxModel>( file.NodeGroupSet.Models, "Models" )
            ];
        }

        public void Reset() => Categories.ForEach( cat => cat.Reset() );

        public void Draw() {
            using var _ = ImRaii.PushId( "##ExportDialog" );

            ImGui.Checkbox( "Export Dependencies", ref ExportDependencies );

            ImGui.SameLine();
            UiUtils.HelpMarker( @"Exports the selected items, as well as any dependencies they have (such as particles depending on textures). It is recommended to leave this selected." );

            ImGui.SameLine();
            if( ImGui.Button( "Reset" ) ) Reset();

            ImGui.SameLine();
            if( ImGui.Button( "Export" ) ) SaveDialog();

            using var child = ImRaii.Child( "Child", new( -1, -1 ), true );
            Categories.ForEach( cat => cat.Draw() );
        }

        public void Show( AvfxNode node ) {
            Plugin.AvfxManager?.ExportDialog.Show();
            Reset();
            foreach( var category in Categories ) {
                if( category.Belongs( node ) ) {
                    category.Select( node );
                    break;
                }
            }
        }

        public List<AvfxNode> GetSelected() {
            var result = new List<AvfxNode>();
            Categories.ForEach( x => result.AddRange( x.GetSelectedNodes() ) );
            return result;
        }

        public void SaveDialog() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".vfxedit2,.*", "ExportedVfx", "vfxedit2", ( bool ok, string res ) => {
                if( !ok ) return;
                AvfxFile.Export( GetSelected(), res, ExportDependencies );
                Plugin.AvfxManager?.ExportDialog.Hide();
            } );
        }
    }
}
