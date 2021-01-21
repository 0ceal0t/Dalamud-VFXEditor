using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Plugin;

namespace VFXEditor.UI.VFX {
    public abstract class UINode : UIItem {
        public static UINodeGroup<UIBinder> _Binders;
        public static UINodeGroup<UIEmitter> _Emitters;
        public static UINodeGroup<UIModel> _Models;
        public static UINodeGroup<UIParticle> _Particles;
        public static UINodeGroup<UIScheduler> _Schedulers;
        public static UINodeGroup<UITexture> _Textures;
        public static UINodeGroup<UITimeline> _Timelines;
        public static UINodeGroup<UIEffector> _Effectors;
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

        public List<UINode> Children = new List<UINode>();
        public List<UINodeSelect> Parents = new List<UINodeSelect>();
        public List<UINodeSelect> Selectors = new List<UINodeSelect>();

        public void DeleteNode() { // NODE DELETED, DON'T NEED ANY FURTHER LOGIC FOR SELECTORS
            foreach( var node in Children ) {
                node.Parents.RemoveAll( x => x.Node == this );
            }
            foreach( var node in Parents ) {
                node.DeleteNode(); // NOTIFY ANY SELECTORS WHICH WERE TARGETING THIS NODE
                node.Node.Children.RemoveAll( x => x == this );
            }

            foreach(var s in Selectors ) {
                s.UnlinkChange();
            }
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

        public void UnlinkFrom(UINode node ) {
            if( node == null ) return;
            Node.Children.Remove( node );
            node.Parents.Remove( this );

            PluginLog.Log( "Unlinking " + Node.GetText() + " from " + node.GetText() );
        }

        public void LinkTo(UINode node ) {
            if( node == null ) return;
            Node.Children.Add( node );
            node.Parents.Add( this );

            PluginLog.Log( "Linking " + Node.GetText() + " to " + node.GetText() );
        }

        public abstract void DeleteSelect(); // when a selector is deleted. call this when deleting an item doesn't delete a node, like EmitterItem
        public abstract void UnlinkChange();
        public abstract void DeleteNode();
        public abstract void UpdateNode();
        public abstract void SetupNode();
        public abstract void Draw( string parentId );
    }
    public class UINodeSelect<T> : UINodeSelect where T : UINode {
        public T Selected = null;
        LiteralInt Literal;
        UINodeGroup<T> Group;
        string Name;

        public UINodeSelect(UINode node, string name, UINodeGroup<T> group, LiteralInt literal){
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

        public override void Draw(string parentId) {
            string id = parentId + "/Node";
            if(ImGui.BeginCombo(Name + id, Selected == null ? "None" : Selected.GetText() ) ) {
                if(ImGui.Selectable("None", Selected == null ) ) {
                    // 
                    UnlinkFrom( Selected );
                    Selected = null;
                    UpdateNode();
                }
                foreach(var item in Group.Items ) {
                    if(ImGui.Selectable(item.GetText(), Selected == item ) ) {
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

        // CALL THIS WHEN THE SELECTOR IS DELETED
        public override void DeleteSelect() {
            UnlinkChange();
            if(Selected != null ) {
                UnlinkFrom( Selected );
            }
        }
        public override void UnlinkChange() {
            Group.OnChange -= UpdateNode;
        }

        public override void UpdateNode() {
            if(Selected != null ) {
                Literal.GiveValue( Selected.Idx );
            }
            else {
                Literal.GiveValue( -1 );
            }
        }

        public override void SetupNode() {
            int val = Literal.Value;
            if(val >= 0 && val < Group.Items.Count ) {
                Selected = Group.Items[val];
                LinkTo( Selected );
            }
        }

        public override void DeleteNode() { // THE SELECTED NODE WAS DELETED
            Selected = null;
            UpdateNode();
        }
    }
}
