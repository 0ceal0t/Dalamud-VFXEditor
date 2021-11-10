using AVFXLib.Models;
using System.Collections.Generic;
using ImGuiNET;
using VFXEditor.Data;

namespace VFXEditor.UI.Vfx {
    public abstract class UINodeSelect : UIBase {
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
    }

    public class UINodeSelect<T> : UINodeSelect where T : UINode {
        public T Selected = null;
        public LiteralInt Literal;
        public UINodeGroup<T> Group;
        public string Name;

        public UINodeSelect( UINode node, string name, UINodeGroup<T> group, LiteralInt literal ) {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.IsInit ) {
                SetupNode();
            }
            else {
                Group.OnInit += SetupNode;
            }
            node.Selectors.Add( this );
        }

        public override void Draw( string parentId ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralInt literal ) {
                Literal.GiveValue( literal.Value );
                UnlinkFrom( Selected );
                if( Literal.Value >= 0 && Literal.Value < Group.Items.Count ) LinkTo(Selected = Group.Items[Literal.Value] );
                else Selected = null;
            }

            // ======= DRAW =========
            var id = parentId + "/Node";
            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) SelectNone();

                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) SelectItem( item );
                    if( ImGui.IsItemHovered() ) item.ShowTooltip();
                }

                ImGui.EndCombo();
            }
        }

        private void SelectNone() {
            if( Selected == null ) return;
            UnlinkFrom( Selected );
            Selected = null;
            UpdateNode();
        }

        private void SelectItem(T item) {
            if( Selected == item ) return;
            UnlinkFrom( Selected );
            LinkTo( item );
            Selected = item;
            UpdateNode();
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
            var val = Literal.Value;
            if( Node.HasDependencies && val >= 0 ) {
                val += Group.PreImportSize;
                Literal.GiveValue( val );
            }
            if( val >= 0 && val < Group.Items.Count ) {
                Selected = Group.Items[val];
                LinkTo( Selected );
            }
        }

        public override void DeleteNode( UINode node ) {
            Selected = null;
            UpdateNode();
        }
    }

    public class UINodeSelectList<T> : UINodeSelect where T : UINode {
        public List<T> Selected = new();
        public LiteralIntList Literal;
        public UINodeGroup<T> Group;
        public string Name;

        public UINodeSelectList( UINode node, string name, UINodeGroup<T> group, LiteralIntList literal ) {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.IsInit ) {
                SetupNode();
            }
            else {
                Group.OnInit += SetupNode;
            }
            node.Selectors.Add( this );
        }

        public override void Draw( string parentId ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralIntList literal ) {
                Literal.GiveValue( literal.Value );
                foreach( var s in Selected ) UnlinkFrom( s );
                Selected.Clear();
                foreach(var item in Literal.Value ) {
                    if( item >= 0 && item < Group.Items.Count ) {
                        Selected.Add( Group.Items[item] );
                        LinkTo( Group.Items[item] );
                    }
                    else Selected.Add( null );
                }
            }

            // ====== DRAW =================
            var id = parentId + "/Node";
            for( var i = 0; i < Selected.Count; i++ ) {
                var _id = id + i;
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
                        if( ImGui.IsItemHovered() ) {
                            item.ShowTooltip();
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
                ImGui.TextColored( UIUtils.RED_COLOR, "WARNING: Add an item!" );
            }
            if(Group.Items.Count == 0 ) {
                ImGui.TextColored( UIUtils.RED_COLOR, "WARNING: Add a selectable item first!" );
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
            var idxs = new List<int>();
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
            for( var i = 0; i < Literal.Value.Count; i++ ) {
                var val = Literal.Value[i];
                if( Node.HasDependencies && val != 255 && val >= 0 ) {
                    val += Group.PreImportSize;
                    Literal.GiveValue( val, i ); 
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
        }

        public override void DeleteNode( UINode node ) {
            Selected.RemoveAll( x => x == node );
            UpdateNode();
        }
    }
}
