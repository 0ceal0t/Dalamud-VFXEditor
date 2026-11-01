using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.AvfxFormat.Binder;
using VfxEditor.Formats.AvfxFormat.Effector;
using VfxEditor.Formats.AvfxFormat.Emitter;
using VfxEditor.Formats.AvfxFormat.Model;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Formats.AvfxFormat.Particle;
using VfxEditor.Formats.AvfxFormat.Texture;
using VfxEditor.Formats.AvfxFormat.Timeline;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AvfxFormat.Dialogs {
    public class AvfxExport {
        public bool IsExporting { get; private set; } = false;
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

            ImGui.SameLine();
            if( ImGui.Button( "Cancel" ) ) {
                IsExporting = false;
            }

            using var child = ImRaii.Child( "Child", new( -1, -1 ), true );
            Categories.ForEach( cat => cat.Draw() );
        }

        public void Show( AvfxNode node ) {
            IsExporting = true;
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
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".vfxedit2,.*", "ExportedVfx", "vfxedit2", ( ok, res ) => {
                if( !ok ) return;
                AvfxFile.Export( GetSelected(), res, ExportDependencies );
                IsExporting = false;
            } );
        }
    }
}
