using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public UIMain(AVFXBase avfx, Plugin plugin)
        {
            AVFX = avfx;
            UINode.SetupGroups();
            // =========================
            ParticleView = new UIParticleView(this, avfx);
            ParameterView = new UIParameterView(avfx);
            BinderView = new UIBinderView( this, avfx );
            EmitterView = new UIEmitterView( this, avfx );
            EffectorView = new UIEffectorView( this, avfx );
            TimelineView = new UITimelineView( this, avfx );
            TextureView = new UITextureView(avfx, plugin);
            ModelView = new UIModelView(avfx, plugin);
            ScheduleView = new UIScheduleView( this, avfx );

            UINode.InitGroups();
        }

        public void Draw()
        {
            if (ImGui.BeginTabBar("##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("Parameters##Main"))
                {
                    ParameterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Scheduler##Main"))
                {
                    ScheduleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Timelines##Main"))
                {
                    TimelineView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Emitters##Main"))
                {
                    EmitterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Particles##Main"))
                {
                    ParticleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Effectors##Main"))
                {
                    EffectorView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Binders##Main"))
                {
                    BinderView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Textures##Main"))
                {
                    TextureView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Models##Main"))
                {
                    ModelView.Draw();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        // ===================
        // export the current node, along with all of its dependencies. this is tricky, since when imported, the indexes are going to be different
        // so when exporting, normalize all of the indexes first
        public byte[] ExportDeps(UINode startNode) {
            List<UINode> nodes = new List<UINode>();
            RecurseChild( startNode, nodes );
            Dictionary<UINode, int> IdxSave = new Dictionary<UINode, int>(); // save these to restore afterwards, since we don't want to modify the current document
            foreach( var n in nodes ) {
                IdxSave[n] = n.Idx;
            }

            FilterByType<UITimeline>( nodes );
            FilterByType<UIEmitter>( nodes );
            FilterByType<UIEffector>( nodes );
            FilterByType<UIBinder>( nodes );
            FilterByType<UIParticle>( nodes );
            FilterByType<UITexture>( nodes );
            FilterByType<UIModel>( nodes );

            UpdateAllNodes( nodes );

            byte[] data;
            using( MemoryStream ms = new MemoryStream() ) {
                using( BinaryWriter bw = new BinaryWriter( ms ) ) {
                    foreach( var n in nodes ) {
                        bw.Write( n.toBytes() );
                    }
                    data = ms.ToArray();
                }
            }

            foreach( var n in nodes ) {
                n.Idx = IdxSave[n];
            }
            UpdateAllNodes( nodes );
            return data;
        }
        public void RecurseChild( UINode node, List<UINode> output ) {
            foreach( var n in node.Children ) {
                RecurseChild( n, output );
            }
            if( output.Contains( node ) ) return;
            output.Add( node );
        }

        public void FilterByType<T>(List<UINode> items) where T : UINode {
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
        public void ImportData( byte[] data ) {
            var nodes = AVFXLib.Main.Reader.readDef( new BinaryReader( new MemoryStream( data ) ) );
            var has_dependencies = nodes.Count >= 2;
            
        }
        public void ProcessNode( AVFXLib.AVFX.AVFXNode node ) {
            switch( node.Name ) {
                case "Tmln":
                    break;
                case "Emit":
                    break;
                case "Efct":
                    break;
                case "Bind":
                    break;
                case "Ptcl":
                    break;
                case "Tex":
                    break;
                case "Modl":
                    AVFXModel model = new AVFXModel();
                    model.read( node );
                    AVFX.addModel( model );
                    break;
            }
        }
    }
}
