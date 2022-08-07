using ImGuiFileDialog;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.Dialogs;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
    public class ExportDialog : GenericDialog {
        private readonly AVFXFile VFXFile;
        private readonly List<ExportDialogCategory> Categories;
        private bool ExportDependencies = true;

        public ExportDialog( AVFXFile main ) : base("Export") {
            VFXFile = main;
            Categories = new List<ExportDialogCategory> {
                new ExportDialogCategory<UITimeline>( main.NodeGroupSet.Timelines, "Timelines" ),
                new ExportDialogCategory<UIEmitter>( main.NodeGroupSet.Emitters, "Emitters" ),
                new ExportDialogCategory<UIParticle>( main.NodeGroupSet.Particles, "Particles" ),
                new ExportDialogCategory<UIEffector>( main.NodeGroupSet.Effectors, "Effectors" ),
                new ExportDialogCategory<UIBinder>( main.NodeGroupSet.Binders, "Binders" ),
                new ExportDialogCategory<UITexture>( main.NodeGroupSet.Textures, "Textures" ),
                new ExportDialogCategory<UIModel>( main.NodeGroupSet.Models, "Models" )
            };
        }

        public void Reset() {
            Categories.ForEach( cat => cat.Reset() );
        }

        public override void DrawBody() {
            ImGui.Checkbox( "Export Dependencies", ref ExportDependencies );
            ImGui.SameLine();
            UIHelper.HelpMarker( @"Exports the selected items, as well as any dependencies they have (such as particles depending on textures). It is recommended to leave this selected." );
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

        public void ShowDialog( UINode node ) {
            Show();
            Reset();
            foreach( var category in Categories ) {
                if( category.Belongs( node ) ) {
                    category.Select( node );
                    break;
                }
            }
        }

        public List<UINode> GetSelected() {
            var result = new List<UINode>();
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
            public HashSet<UINode> Selected;
            public abstract void Reset();
            public abstract void Draw();
            public abstract bool Belongs( UINode node );
            public abstract void Select( UINode node );
        }

        private class ExportDialogCategory<T> : ExportDialogCategory where T : UINode {
            public UINodeGroup<T> Group;
            public string HeaderText;
            public string Id;

            public ExportDialogCategory( UINodeGroup<T> group, string text ) {
                Group = group;
                Reset();
                Group.OnChange += Reset;
                HeaderText = text;
                Id = "##" + HeaderText;
            }

            public override void Reset() {
                Selected = new HashSet<UINode>();
            }

            public override bool Belongs( UINode node ) {
                return node is T;
            }

            public override void Select( UINode node ) {
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
