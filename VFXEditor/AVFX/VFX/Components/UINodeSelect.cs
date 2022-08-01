using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.Data;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
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
        public abstract void DeleteNode( UINode node ); // when the selected node is deleted
        public abstract void UpdateNode();
        public abstract void SetupNode();
    }

    public class UINodeSelect<T> : UINodeSelect where T : UINode {
        public T Selected = null;
        public AVFXInt Literal;
        public UINodeGroup<T> Group;
        public string Name;

        public UINodeSelect( UINode node, string name, UINodeGroup<T> group, AVFXInt literal ) : base() {
            Node = node;
            Name = name;
            Group = group;
            Literal = literal;
            Group.OnChange += UpdateNode;
            if( Group.IsInitialized ) {
                SetupNode();
            }
            else {
                Group.OnInit += SetupNode;
            }
            node.Selectors.Add( this );
        }

        public override void Draw( string parentId ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is AVFXInt literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                UnlinkFrom( Selected );
                if( Literal.GetValue() >= 0 && Literal.GetValue() < Group.Items.Count ) LinkTo( Selected = Group.Items[Literal.GetValue()] );
                else Selected = null;
            }

            // ======= DRAW =========
            var id = parentId + "/Node";

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) SelectNone();

                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) SelectItem( item );
                    if( ImGui.IsItemHovered() ) item.ShowTooltip();
                }

                ImGui.EndCombo();
            }

            if( DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }

        private void SelectNone() {
            if( Selected == null ) return;
            UnlinkFrom( Selected );
            Selected = null;
            UpdateNode();
        }

        private void SelectItem( T item ) {
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

        public override void UpdateNode() => Literal.SetValue( Selected != null ? Selected.Idx : -1 );

        public override void SetupNode() {
            var val = Literal.GetValue();
            if( Node.HasDependencies && val >= 0 ) {
                val += Group.PreImportSize;
                Literal.SetValue( val );
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
}
