using ImGuiFileDialog;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Dialogs {
    public class ExportDialog : GenericDialog {
        private readonly AvfxFile VfxFile;
        private readonly List<ExportDialogCategory> Categories;
        private bool ExportDependencies = true;

        public ExportDialog( AvfxFile vfxFile ) : base( "Export", false, 600, 400 ) {
            VfxFile = vfxFile;
            Categories = new List<ExportDialogCategory> {
                new ExportDialogCategory<AvfxTimeline>( vfxFile.NodeGroupSet.Timelines, "Timelines" ),
                new ExportDialogCategory<AvfxEmitter>( vfxFile.NodeGroupSet.Emitters, "Emitters" ),
                new ExportDialogCategory<AvfxParticle>( vfxFile.NodeGroupSet.Particles, "Particles" ),
                new ExportDialogCategory<AvfxEffector>( vfxFile.NodeGroupSet.Effectors, "Effectors" ),
                new ExportDialogCategory<AvfxBinder>( vfxFile.NodeGroupSet.Binders, "Binders" ),
                new ExportDialogCategory<AvfxTexture>( vfxFile.NodeGroupSet.Textures, "Textures" ),
                new ExportDialogCategory<AvfxModel>( vfxFile.NodeGroupSet.Models, "Models" )
            };
        }

        public void Reset() => Categories.ForEach( cat => cat.Reset() );

        public override void DrawBody() {
            ImGui.Checkbox( "Export Dependencies", ref ExportDependencies );

            ImGui.SameLine();
            UiUtils.HelpMarker( @"Exports the selected items, as well as any dependencies they have (such as particles depending on textures). It is recommended to leave this selected." );

            ImGui.SameLine();
            if( ImGui.Button( "Reset##ExportDialog" ) ) Reset();

            ImGui.SameLine();
            if( ImGui.Button( "Export##ExportDialog" ) ) SaveDialog();

            ImGui.BeginChild( "##ExportRegion", ImGui.GetContentRegionAvail(), false );
            Categories.ForEach( cat => cat.Draw() );
            ImGui.EndChild();
        }

        public void ShowDialog( AvfxNode node ) {
            Show();
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
            foreach( var category in Categories ) {
                result.AddRange( category.Selected );
            }
            return result;
        }

        public void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxedit2,.*", "ExportedVfx", "vfxedit2", ( bool ok, string res ) => {
                if( !ok ) return;
                VfxFile.Export( GetSelected(), res, ExportDependencies );
                Visible = false;
            } );
        }
    }
}
