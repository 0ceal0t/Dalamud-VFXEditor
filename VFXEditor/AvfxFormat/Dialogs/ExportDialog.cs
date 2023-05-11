using ImGuiFileDialog;
using ImGuiNET;
using Microsoft.VisualBasic;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.Ui.Nodes;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
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

        public void Reset() {
            Categories.ForEach( cat => cat.Reset() );
        }

        public override void DrawBody() {
            ImGui.Checkbox( "Export Dependencies", ref ExportDependencies );
            ImGui.SameLine();
            UiUtils.HelpMarker( @"Exports the selected items, as well as any dependencies they have (such as particles depending on textures). It is recommended to leave this selected." );
            ImGui.SameLine();
            if( ImGui.Button( "Reset##ExportDialog" ) ) {
                Reset();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Export##ExportDialog" ) ) {
                SaveDialog();
            }

            var maxSize = ImGui.GetContentRegionAvail();
            ImGui.BeginChild( "##ExportRegion", maxSize, false );

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

        private abstract class ExportDialogCategory {
            public HashSet<AvfxNode> Selected;
            public abstract void Reset();
            public abstract void Draw();
            public abstract bool Belongs( AvfxNode node );
            public abstract void Select( AvfxNode node );
        }

        private class ExportDialogCategory<T> : ExportDialogCategory where T : AvfxNode {
            public NodeGroup<T> Group;
            public string HeaderText;
            public string Id;

            public ExportDialogCategory( NodeGroup<T> group, string text ) {
                Group = group;
                Reset();
                Group.OnChange += Reset;
                HeaderText = text;
                Id = "##" + HeaderText;
            }

            public override void Reset() {
                Selected = new HashSet<AvfxNode>();
            }

            public override bool Belongs( AvfxNode node ) => node is T;

            public override void Select( AvfxNode node ) => Selected.Add( node );

            public override void Draw() {
                ImGui.SetNextItemOpen( false, ImGuiCond.FirstUseEver );

                using var color = ImRaii.PushColor( ImGuiCol.Text, new Vector4( 0.10f, 0.90f, 0.10f, 1.0f ), Selected.Count > 0 );
                if( ImGui.CollapsingHeader( $"{HeaderText} ({Selected.Count} Selected / {Group.Items.Count})###ExportUI/{HeaderText}" ) ) {
                    color.Pop();

                    using var indent = ImRaii.PushIndent();
                    foreach( var item in Group.Items ) {
                        var nodeSelected = Selected.Contains( item );
                        if( ImGui.Checkbox( item.GetText() + Id, ref nodeSelected ) ) {
                            if( nodeSelected ) Selected.Add( item );
                            else Selected.Remove( item );
                        }
                    }
                }
            }
        }
    }
}
