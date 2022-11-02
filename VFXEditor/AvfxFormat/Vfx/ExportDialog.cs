using ImGuiFileDialog;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Dialogs;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class ExportDialog : GenericDialog {
        private readonly AvfxFile VFXFile;
        private readonly List<ExportDialogCategory> Categories;
        private bool ExportDependencies = true;

        public ExportDialog( AvfxFile main ) : base("Export") {
            VFXFile = main;
            Categories = new List<ExportDialogCategory> {
                new ExportDialogCategory<UiTimeline>( main.NodeGroupSet.Timelines, "Timelines" ),
                new ExportDialogCategory<UiEmitter>( main.NodeGroupSet.Emitters, "Emitters" ),
                new ExportDialogCategory<UiParticle>( main.NodeGroupSet.Particles, "Particles" ),
                new ExportDialogCategory<UiEffector>( main.NodeGroupSet.Effectors, "Effectors" ),
                new ExportDialogCategory<UiBinder>( main.NodeGroupSet.Binders, "Binders" ),
                new ExportDialogCategory<UiTexture>( main.NodeGroupSet.Textures, "Textures" ),
                new ExportDialogCategory<UiModel>( main.NodeGroupSet.Models, "Models" )
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

        public void ShowDialog( UiNode node ) {
            Show();
            Reset();
            foreach( var category in Categories ) {
                if( category.Belongs( node ) ) {
                    category.Select( node );
                    break;
                }
            }
        }

        public List<UiNode> GetSelected() {
            var result = new List<UiNode>();
            foreach( var category in Categories ) {
                result.AddRange( category.Selected );
            }
            return result;
        }

        public void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxedit2,.*", "ExportedVfx", "vfxedit2", ( bool ok, string res ) => {
                if( !ok ) return;
                VFXFile.Export( GetSelected(), res, ExportDependencies );
                Visible = false;
            } );
        }

        private abstract class ExportDialogCategory {
            public HashSet<UiNode> Selected;
            public abstract void Reset();
            public abstract void Draw();
            public abstract bool Belongs( UiNode node );
            public abstract void Select( UiNode node );
        }

        private class ExportDialogCategory<T> : ExportDialogCategory where T : UiNode {
            public UiNodeGroup<T> Group;
            public string HeaderText;
            public string Id;

            public ExportDialogCategory( UiNodeGroup<T> group, string text ) {
                Group = group;
                Reset();
                Group.OnChange += Reset;
                HeaderText = text;
                Id = "##" + HeaderText;
            }

            public override void Reset() {
                Selected = new HashSet<UiNode>();
            }

            public override bool Belongs( UiNode node ) {
                return node is T;
            }

            public override void Select( UiNode node ) {
                Selected.Add( node );
            }

            public override void Draw() {
                ImGui.SetNextItemOpen( false, ImGuiCond.FirstUseEver );

                var count = Selected.Count;
                var visible = false;
                if( count > 0 ) {
                    ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.10f, 0.90f, 0.10f, 1.0f ) );
                }
                if( ImGui.CollapsingHeader( $"{HeaderText} ({count} Selected / {Group.Items.Count})###ExportUI_{HeaderText}" ) ) {
                    if( count > 0 ) {
                        visible = true;
                        ImGui.PopStyleColor();
                    }

                    ImGui.Indent();

                    foreach( var item in Group.Items ) {
                        var _selected = Selected.Contains( item );
                        if( ImGui.Checkbox( item.GetText() + Id, ref _selected ) ) {
                            if( _selected ) {
                                Selected.Add( item );
                            }
                            else {
                                Selected.Remove( item );
                            }
                        }
                    }


                    ImGui.Unindent();
                }
                if( count > 0 && !visible ) {
                    ImGui.PopStyleColor();
                }
            }
        }
    }
}
