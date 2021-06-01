using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFXEditor.UI.VFX.Main;

namespace VFXEditor.UI.VFX
{
    public class UIMain
    {
        public AVFXBase AVFX;

        public UIParameterView ParameterView;
        public UIEffectorView EffectorView;
        public UIEmitterView EmitterView;
        public UIModelView ModelView;
        public UIParticleView ParticleView;
        public UITextureView TextureView;
        public UITimelineView TimelineView;
        public UIScheduleView ScheduleView;
        public UIBinderView BinderView;

        public ExportDialog ExportUI;

        public UIMain(AVFXBase avfx, Plugin plugin) {
            AVFX = avfx;
            // ================
            UINodeGroup.SetupGroups();
            ParticleView = new UIParticleView(this, avfx);
            ParameterView = new UIParameterView(avfx);
            BinderView = new UIBinderView( this, avfx );
            EmitterView = new UIEmitterView( this, avfx );
            EffectorView = new UIEffectorView( this, avfx );
            TimelineView = new UITimelineView( this, avfx );
            TextureView = new UITextureView(avfx, plugin);
            ModelView = new UIModelView(this, avfx, plugin);
            ScheduleView = new UIScheduleView( this, avfx );
            UINodeGroup.InitGroups();
            // =================
            ExportUI = new ExportDialog( this );
        }

        public void Draw() {
            if (ImGui.BeginTabBar("##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("Parameters##Main")) {
                    ParameterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Scheduler##Main")) {
                    ScheduleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Timelines##Main")) {
                    TimelineView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Emitters##Main")) {
                    EmitterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Particles##Main")) {
                    ParticleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Effectors##Main")) {
                    EffectorView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Binders##Main")) {
                    BinderView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Textures##Main")) {
                    TextureView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Models##Main")) {
                    ModelView.Draw();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            // =========
            ExportUI.Draw();
        }

        // ========= EXPORT ==============
        public void ExportDeps(UINode startNode, BinaryWriter bw ) {
            ExportDeps(new List<UINode>(new []{ startNode } ), bw);
        }
        public void ExportDeps(List<UINode> startNodes, BinaryWriter bw) {
            HashSet<UINode> visited = new HashSet<UINode>();
            List<UINode> nodes = new List<UINode>();
            foreach(var startNode in startNodes ) {
                RecurseChild( startNode, nodes, visited );
            }

            Dictionary<UINode, int> IdxSave = new Dictionary<UINode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxSave[n] = n.Idx;
            }

            OrderByType<UITimeline>( nodes );
            OrderByType<UIEmitter>( nodes );
            OrderByType<UIEffector>( nodes );
            OrderByType<UIBinder>( nodes );
            OrderByType<UIParticle>( nodes );
            OrderByType<UITexture>( nodes );
            OrderByType<UIModel>( nodes );

            UpdateAllNodes( nodes );
            foreach( var n in nodes ) {
                bw.Write( n.ToBytes() );
            }
            foreach( var n in nodes ) {
                n.Idx = IdxSave[n];
            }
            UpdateAllNodes( nodes );
        }
        public void RecurseChild( UINode node, List<UINode> output, HashSet<UINode> visited ) {
            if( visited.Contains( node ) ) return; // prevents infinite loop
            visited.Add( node );

            foreach( var n in node.Children ) {
                RecurseChild( n, output, visited );
            }
            if( output.Contains( node ) ) return; // make sure elements get added AFTER their children. This doesn't work otherwise, since we want each node to be AFTER its dependencies
            output.Add( node );
        }
        public void OrderByType<T>(List<UINode> items) where T : UINode {
            int i = 0;
            foreach(UINode node in items ) {
                if (node is T ) {
                    node.Idx = i;
                    i++;
                }
            }
        }
        public void UpdateAllNodes( List<UINode> nodes ) {
            foreach( var n in nodes ) {
                foreach( var s in n.Selectors ) {
                    s.UpdateNode();
                }
            }
        }

        // ========= IMPORT ==============
        public void ImportData(string path ) {
            using( BinaryReader reader = new BinaryReader( File.Open( path, FileMode.Open ) ) ) {
                ImportData( reader );
            }
        }
        public void ImportData(byte[] data ) {
            ImportData( new BinaryReader( new MemoryStream( data ) ) );
        }
        public void ImportData( BinaryReader br ) {
            var messages = new List<string>();
            var nodes = AVFXLib.Main.Reader.ReadDefinition( br, messages);
            var has_dependencies = nodes.Count >= 2;
            if( has_dependencies ) {
                UINodeGroup.PreImportGroups();
            }
            nodes.Where( x => x.Name == "Modl" ).ToList().ForEach( node => ModelView.Group.Add(ModelView.OnImport( node )) );
            nodes.Where( x => x.Name == "Tex" ).ToList().ForEach( node => TextureView.Group.Add(TextureView.OnImport( node )) );
            nodes.Where( x => x.Name == "Bind" ).ToList().ForEach( node => BinderView.Group.Add(BinderView.OnImport( node, has_dependencies )) );
            nodes.Where( x => x.Name == "Efct" ).ToList().ForEach( node => EffectorView.Group.Add(EffectorView.OnImport( node, has_dependencies )) );
            nodes.Where( x => x.Name == "Ptcl" ).ToList().ForEach( node => ParticleView.Group.Add(ParticleView.OnImport( node, has_dependencies )) );
            nodes.Where( x => x.Name == "Emit" ).ToList().ForEach( node => EmitterView.Group.Add(EmitterView.OnImport( node, has_dependencies )) );
            nodes.Where( x => x.Name == "TmLn" ).ToList().ForEach( node => TimelineView.Group.Add(TimelineView.OnImport( node, has_dependencies )) );
        }

        // =========== DIALOG =================

        public void ExportMultiple(UINode node ) {
            ExportUI.Export( node );
        }

        public static void ExportDialog(UINode node ) {
            Task.Run( async () => {
                var picker = new SaveFileDialog {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "vfxedit",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        File.WriteAllBytes( picker.FileName, node.ToBytes() );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }

        public void ImportDialog() {
            Task.Run( async () => {
                var picker = new OpenFileDialog {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a File Location.",
                    CheckFileExists = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        ImportData( picker.FileName );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }
    }
}
