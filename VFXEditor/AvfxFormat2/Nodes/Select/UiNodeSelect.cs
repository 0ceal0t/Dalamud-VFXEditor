using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public abstract class UiNodeSelect : IUiBase {
        public readonly AvfxNode Node;

        public UiNodeSelect( AvfxNode node ) {
            Node = node;
        }

        public abstract void OnChange();
        public abstract void Initialize();

        public abstract void LinkOnChange();
        public abstract void UnlinkOnChange();

        public abstract void Draw( string id );

        // For when something happens to the selector

        public abstract void Enable();
        public abstract void Disable();

        // For when something happens to the selected node

        public abstract List<int> GetSelectedIdx( AvfxNode node );
        public abstract void EnableNode( AvfxNode node, int idx );
        public abstract void DisableNode( AvfxNode node );

        public void UnlinkParentChild( AvfxNode node ) {
            if( node == null ) return;
            Node.ChildNodes.Remove( node );
            node.Parents.Remove( this );

            node.Graph?.NowOutdated();
        }

        public void LinkParentChild( AvfxNode node ) {
            if( node == null ) return;
            Node.ChildNodes.Add( node );
            node.Parents.Add( this );

            node.Graph?.NowOutdated();
        }
    }

    public class UiNodeSelect<T> : UiNodeSelect where T : AvfxNode {
        public T Selected = null;
        public readonly AvfxInt Literal;
        public readonly UiNodeGroup<T> Group; // the group being selected from
        public readonly string Name;

        private bool Enabled = true;

        public UiNodeSelect( AvfxNode node, string name, UiNodeGroup<T> group, AvfxInt literal ) : base( node ) {
            Name = name;
            Group = group;
            Literal = literal;
            LinkOnChange();
            if( Group.IsInitialized ) Initialize(); // already good to go
            else Group.OnInit += Initialize;
            node.Selectors.Add( this );
        }

        public override void OnChange() => Literal.SetValue( Selected != null ? Selected.GetIdx() : -1 ); // an item was removed from the group, for example

        public override void Initialize() {
            var value = Literal.GetValue();
            if( Node.HasDependencies && value >= 0 ) {
                value += Group.PreImportSize;
                Literal.SetValue( value );
            }
            if( value >= 0 && value < Group.Items.Count ) {
                Selected = Group.Items[value];
                LinkParentChild( Selected );
            }
        }

        public void Select( T item ) {
            if( item == null ) SelectNone();
            else SelectItem( item );
        }

        private void SelectNone() {
            if( Selected == null ) return;
            UnlinkParentChild( Selected );
            Selected = null;
            OnChange();
        }

        private void SelectItem( T item ) {
            if( Selected == item ) return;
            UnlinkParentChild( Selected );
            LinkParentChild( item );
            Selected = item;
            OnChange();
        }

        public override void LinkOnChange() { Group.OnChange += OnChange; }

        public override void UnlinkOnChange() { Group.OnChange -= OnChange; }

        // For when something happens to the selector

        public override void Enable() {
            if( Enabled ) return;
            Enabled = true;
            LinkOnChange();
            if( Selected != null ) LinkParentChild( Selected );
        }

        public override void Disable() {
            if( !Enabled ) return;
            Enabled = false;
            UnlinkOnChange();
            if( Selected != null ) UnlinkParentChild( Selected );
        }

        // For when something happens to the selected node

        public override List<int> GetSelectedIdx( AvfxNode node ) => null; // not used for this one, since there is only every 1 selector

        public override void EnableNode( AvfxNode node, int _ ) {
            Selected = ( T )node;
            OnChange();
        }

        public override void DisableNode( AvfxNode node ) {
            Selected = null;
            OnChange();
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Node";

            // Unassigned
            if( AvfxBase.DrawAddButton( Literal, Name, id ) ) return;

            if( ImGui.BeginCombo( Name + id, Selected == null ? "[NONE]" : Selected.GetText() ) ) {
                // "None" selector
                if( ImGui.Selectable( "[NONE]", Selected == null ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, null ) );

                foreach( var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) CommandManager.Avfx.Add( new UiNodeSelectCommand<T>( this, item ) );
                    if( ImGui.IsItemHovered() ) item.ShowTooltip();
                }
                ImGui.EndCombo();
            }

            AvfxBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
