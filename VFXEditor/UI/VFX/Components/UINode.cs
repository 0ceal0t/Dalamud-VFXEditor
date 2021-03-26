using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Plugin;
using System.Numerics;

namespace VFXEditor.UI.VFX {
    public abstract class UINode : UIItem {
        public static UINodeGroup<UIBinder> _Binders;
        public static uint BinderColor = ImGui.GetColorU32( new Vector4( 0.17f, 0.54f, 0.36f, 1 ) );

        public static UINodeGroup<UIEmitter> _Emitters;
        public static uint EmitterColor = ImGui.GetColorU32( new Vector4( 0.62f, 0.37f, 0.20f, 1 ) );

        public static UINodeGroup<UIModel> _Models;
        public static uint ModelColor = ImGui.GetColorU32( new Vector4( 0.49f, 0.17f, 0.19f, 1 ) );

        public static UINodeGroup<UIParticle> _Particles;
        public static uint ParticleColor = ImGui.GetColorU32( new Vector4( 0.42f, 0.34f, 0.68f, 1 ) );

        public static UINodeGroup<UIScheduler> _Schedulers;
        public static uint SchedColor = ImGui.GetColorU32( new Vector4( 0.69f, 0.12f, 0.36f, 1 ) );

        public static UINodeGroup<UITexture> _Textures;
        public static uint TextureColor = ImGui.GetColorU32( new Vector4( 0.40f, 0.69f, 0.12f, 1 ) );

        public static UINodeGroup<UITimeline> _Timelines;
        public static uint TimelineColor = ImGui.GetColorU32( new Vector4( 0.29f, 0.53f, 0.69f, 1 ) );

        public static UINodeGroup<UIEffector> _Effectors;
        public static uint EffectorColor = ImGui.GetColorU32( new Vector4( 0.26f, 0.24f, 0.82f, 1 ) );


        public static void SetupGroups() {
            _Binders = new UINodeGroup<UIBinder>();
            _Emitters = new UINodeGroup<UIEmitter>();
            _Models = new UINodeGroup<UIModel>();
            _Particles = new UINodeGroup<UIParticle>();
            _Schedulers = new UINodeGroup<UIScheduler>();
            _Textures = new UINodeGroup<UITexture>();
            _Timelines = new UINodeGroup<UITimeline>();
            _Effectors = new UINodeGroup<UIEffector>();
        }
        public static void InitGroups() {
            _Binders?.Init();
            _Emitters?.Init();
            _Models?.Init();
            _Particles?.Init();
            _Schedulers?.Init();
            _Textures?.Init();
            _Timelines?.Init();
            _Effectors?.Init();
        }

        public uint _Color;
        public List<UINode> Children = new List<UINode>();
        public List<UINodeSelect> Parents = new List<UINodeSelect>();
        public List<UINodeSelect> Selectors = new List<UINodeSelect>();
        public bool Imported;

        public UINodeGraph Graph = null;

        public void DeleteNode() {
            foreach( var node in Children ) {
                node.Parents.RemoveAll( x => x.Node == this );

                node.Graph?.NowOutdated();
            }
            foreach( var node in Parents ) {
                node.DeleteNode(this);
                node.Node.Children.RemoveAll( x => x == this );
            }

            foreach( var s in Selectors ) {
                s.UnlinkChange();
            }
        }

        public void RefreshGraph() {
            Graph = new UINodeGraph( this );
        }

        public bool HasToolTip = false;
        public virtual void DrawToolTip() { }
        public abstract byte[] toBytes();
    }

    public class UINodeGraphItem {
        public int Level;
        public int Level2;
        public List<UINode> Next;
    }
    public class UINodeGraph {
        public Dictionary<UINode, UINodeGraphItem> Graph = new Dictionary<UINode, UINodeGraphItem>();
        public bool Outdated = false;
        public bool Cycle = false;

        public UINodeGraph( UINode node) {
            ParseGraph( 0, node, new HashSet<UINode>() );
            Dictionary<int, int> L2Dict = new Dictionary<int, int>();
            foreach(var val in Graph.Values ) {
                if( L2Dict.ContainsKey( val.Level ) ) {
                    L2Dict[val.Level] += 1;
                    val.Level2 = L2Dict[val.Level];
                }
                else {
                    L2Dict[val.Level] = 0;
                    val.Level2 = 0;
                }
            }
        }

        public void ParseGraph(int level, UINode node, HashSet<UINode> visited ) {
            if(visited.Contains(node) || Cycle ) {
                Cycle = true;
                return;
            }
            if( Graph.ContainsKey( node ) ) { // already defined
                if(level > Graph[node].Level ) {
                    PushBack( node, level - Graph[node].Level );
                }
                Graph[node].Level = Math.Max( level, Graph[node].Level );
            }
            else {
                visited.Add( node );
                UINodeGraphItem item = new UINodeGraphItem();
                item.Level = level;
                item.Next = new List<UINode>();
                foreach(var n in node.Parents ) {
                    item.Next.Add( n.Node );
                    ParseGraph( level + 1, n.Node, new HashSet<UINode>( visited ) );
                }
                Graph[node] = item;
            }
        }

        public void PushBack(UINode node, int amount ) {
            Graph[node].Level += amount;
            foreach(var item in Graph[node].Next ) {
                PushBack( item, amount );
            }
        }

        public void NowOutdated() {
            Outdated = true;
        }
    }

    public class UINodeGroup<T> where T : UINode {
        public List<T> Items = new List<T>();
        public Action OnInit;
        public Action OnChange;
        public bool isInit = false;

        public UINodeGroup() {
        }

        public void Remove( T item ) {
            item.Idx = -1;
            Items.Remove( item );
            UpdateIdx();
            Update();
        }

        public void Add( T item ) {
            item.Idx = Items.Count;
            Items.Add( item );
        }

        public void Update() {
            OnChange?.Invoke();
        }

        public void Init() {
            UpdateIdx();
            isInit = true;
            OnInit?.Invoke();
            OnInit = null;
        }

        public void UpdateIdx() {
            for( int i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }
    }

    public abstract class UINodeSelect {
        public UINode Node;

        public void UnlinkFrom( UINode node ) {
            if( node == null ) return;
            Node.Children.Remove( node );
            node.Parents.Remove( this );

            node.Graph?.NowOutdated();
        }

        public void LinkTo( UINode node ) {
            if( node == null ) return;
            Node.Children.Add( node );
            node.Parents.Add( this );

            node.Graph?.NowOutdated();
        }

        public abstract void DeleteSelect(); // when a selector is deleted. call this when deleting an item doesn't delete a node, like EmitterItem
        public abstract void UnlinkChange();
        public abstract void DeleteNode(UINode node); // when the selected node is deleted
        public abstract void UpdateNode();
        public abstract void SetupNode();
        public abstract void Draw( string parentId );
    }

    public class UINodeSelect<T> : UINodeSelect where T : UINode {
        public T Selected = null;
        LiteralInt Literal;
        UINodeGroup<T> Group;
        string Name;

        public UINodeSelect( UINode node, string name, UINodeGroup<T> group, LiteralInt literal ) {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.isInit ) {
                SetupNode();
            }
            else {
                Group.OnInit += SetupNode;
            }
            node.Selectors.Add( this );
        }

        public override void Draw( string parentId ) {
            string id = parentId + "/Node";
            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) {
                    // 
                    UnlinkFrom( Selected );
                    Selected = null;
                    UpdateNode();
                }
                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                        //
                        UnlinkFrom( Selected );
                        LinkTo( item );
                        Selected = item;
                        UpdateNode();
                    }
                }
                ImGui.EndCombo();
            }
        }

        public override void DeleteSelect() {
            UnlinkChange();
            if( Selected != null ) {
                UnlinkFrom( Selected );
            }
        }
        public override void UnlinkChange() {
            Group.OnChange -= UpdateNode;
        }

        public override void UpdateNode() {
            if( Selected != null ) {
                Literal.GiveValue( Selected.Idx );
            }
            else {
                Literal.GiveValue( -1 );
            }
        }

        public override void SetupNode() {
            int val = Literal.Value;
            if( Node.Imported && val > 0 ) {
                val += Group.Items.Count;
                Literal.GiveValue( val );
                PluginLog.Log( $"Imported node {val}" );
            }
            if( val >= 0 && val < Group.Items.Count ) {
                Selected = Group.Items[val];
                LinkTo( Selected );
            }
            Node.Imported = false;
        }

        public override void DeleteNode( UINode node ) {
            Selected = null;
            UpdateNode();
        }
    }

    public class UINodeSelectList<T> : UINodeSelect where T : UINode {
        public List<T> Selected = new List<T>();
        LiteralIntList Literal;
        UINodeGroup<T> Group;
        string Name;

        public UINodeSelectList( UINode node, string name, UINodeGroup<T> group, LiteralIntList literal ) {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.isInit ) {
                SetupNode();
            }
            else {
                Group.OnInit += SetupNode;
            }
            node.Selectors.Add( this );
        }

        public override void Draw( string parentId ) {
            string id = parentId + "/Node";
            for(int i = 0; i < Selected.Count; i++ ) {
                string _id = id + i;
                var text = ( i == 0 ) ? Name : "";
                if(ImGui.BeginCombo(text + _id, Selected[i] == null ? "[NONE]" : Selected[i].GetText() ) ) {
                    if( ImGui.Selectable( "[NONE]", Selected[i] == null ) ) {
                        UnlinkFrom( Selected[i] );
                        Selected[i] = null;
                        UpdateNode();
                    }
                    foreach(var item in Group.Items ) {
                        if(ImGui.Selectable(item.GetText(), Selected[i] == item ) ) {
                            UnlinkFrom( Selected[i] );
                            LinkTo( item );
                            Selected[i] = item;
                            UpdateNode();
                        }
                    }
                    ImGui.EndCombo();
                }
                if(i > 0 ) {
                    ImGui.SameLine();
                    if(UIUtils.RemoveButton("- Remove" + _id, small: true ) ) {
                        UnlinkFrom( Selected[i] );
                        Selected.RemoveAt( i );
                        return;
                    }
                }
            }
            if(Selected.Count == 0 ) {
                ImGui.Text( Name );
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "WARNING: Add an item!" );
            }
            if(Group.Items.Count == 0 ) {
                ImGui.TextColored(new Vector4(1,0,0,1), "WARNING: Add a selectable item first!" );
            }
            if( Selected.Count < 4 ) {
                if( ImGui.SmallButton( "+ " + Name + id ) ) {
                    Selected.Add( Group.Items[0] );
                    LinkTo( Group.Items[0] );
                }
            }
        }

        public override void DeleteSelect() {
            UnlinkChange();
            foreach(var item in Selected ) {
                UnlinkFrom( item );
            }
        }
        public override void UnlinkChange() {
            Group.OnChange -= UpdateNode;
        }

        public override void UpdateNode() {
            List<int> idxs = new List<int>();
            foreach(var item in Selected ) {
                if( item == null ) {
                    idxs.Add( 255 );
                }
                else {
                    idxs.Add( item.Idx );
                }
            }
            Literal.GiveValue( idxs );
        }

        public override void SetupNode() {
            for(int i = 0; i < Literal.Value.Count; i++ ) {
                var val = Literal.Value[i];
                if( Node.Imported && val != 255 ) {
                    val += Group.Items.Count;
                    Literal.GiveValue( val, i );
                    PluginLog.Log( $"Imported list {val}" );
                }
                if( val != 255 && val >= 0 && val < Group.Items.Count ) {
                    var item = Group.Items[val];
                    Selected.Add( item );
                    LinkTo( item );
                }
                else {
                    Selected.Add( null );
                }
            }
            Node.Imported = false;
        }

        public override void DeleteNode( UINode node ) {
            Selected.RemoveAll( x => x == node );
            UpdateNode();
        }
    }
}
