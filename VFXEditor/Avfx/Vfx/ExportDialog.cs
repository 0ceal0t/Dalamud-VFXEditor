using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using VFXEditor.Helper;
using VFXEditor.Avfx.Vfx;

namespace VFXEditor.Avfx.Vfx {
    public class ExportDialog {
        public AvfxFile Main;
        private readonly List<ExportDialogCategory> Categories;

        public bool Visible = false;
        public bool ExportDeps = true;

        public ExportDialog( AvfxFile main ) {
            Main = main;
            Categories = new List<ExportDialogCategory> {
                new ExportDialogCategory<UITimeline>( main.Timelines, "Timelines" ),
                new ExportDialogCategory<UIEmitter>( main.Emitters, "Emitters" ),
                new ExportDialogCategory<UIParticle>( main.Particles, "Particles" ),
                new ExportDialogCategory<UIEffector>( main.Effectors, "Effectors" ),
                new ExportDialogCategory<UIBinder>( main.Binders, "Binders" ),
                new ExportDialogCategory<UITexture>( main.Textures, "Textures" ),
                new ExportDialogCategory<UIModel>( main.Models, "Models" )
            };
        }

        public void Show() {
            Visible = true;
        }

        public void Reset() {
            Categories.ForEach( cat => cat.Reset() );
        }

        public void Draw() {
            if( !Visible ) return;
            ImGui.SetNextWindowSize( new Vector2( 500, 500 ), ImGuiCond.FirstUseEver );
            if( ImGui.Begin( "Export##ExportDialog", ref Visible ) ) {
                ImGui.Checkbox( "Export Dependencies", ref ExportDeps );
                ImGui.SameLine();
                UiHelper.HelpMarker( @"Exports the selected items, as well as any dependencies they have (such as particles depending on textures). It is recommended to leave this selected." );
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
            ImGui.End();
        }

        public void Export( UINode node ) {
            Show();
            Reset();
            foreach( var cat in Categories ) {
                if( cat.Belongs( node ) ) {
                    cat.Select( node );
                    break;
                }
            }
        }

        public List<UINode> GetSelected() {
            var result = new List<UINode>();
            foreach( var cat in Categories ) {
                result.AddRange( cat.Selected );
            }
            return result;
        }

        public void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".vfxedit,.*", "ExportedVfx", "vfxedit", ( bool ok, string res ) => {
                if( !ok ) return;
                using var writer = new BinaryWriter( File.Open( res, FileMode.Create ) );
                var selected = GetSelected();
                if( ExportDeps ) {
                    Main.ExportDeps( selected, writer );
                }
                else {
                    selected.ForEach( node => writer.Write( node.ToBytes() ) );
                }
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
                var _visible = false;
                if( count > 0 ) {
                    ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.10f, 0.90f, 0.10f, 1.0f ) );
                }
                if( ImGui.CollapsingHeader( $"{HeaderText} ({count} Selected / {Group.Items.Count})###ExportUI_{HeaderText}" ) ) {
                    if( count > 0 ) {
                        _visible = true;
                        ImGui.PopStyleColor();
                    }

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
                }
                if( count > 0 && !_visible ) {
                    ImGui.PopStyleColor();
                }
            }
        }
    }
}
